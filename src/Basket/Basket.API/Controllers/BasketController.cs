using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using EventBusRabbitMQ.Common;
using EventBusRabbitMQ.Events;
using EventBusRabbitMQ.Producers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Basket.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _repository;
        private readonly ILogger<BasketController> _logger;
        private readonly EventBusRabbitMQProducer _eventBus;
        private readonly IMapper _mapper;

        public BasketController(IBasketRepository repository, EventBusRabbitMQProducer eventBus, IMapper mapper, ILogger<BasketController> logger)
        {
            _repository = repository;
            _eventBus = eventBus;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(BasketCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BasketCart>> GetBasket(string username)
        {
            var basket = await _repository.GetBasket(username);
            return Ok(basket ?? new BasketCart(username));
        }

        [HttpDelete("{username}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> DeleteBasket(string username)
        {
            return Ok(await _repository.DeleteBasket(username));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete_from_Basket_item(string username, BasketCartItem item_to_delete)
        {
            if (string.IsNullOrEmpty(username)) return BadRequest();
            return Ok(await _repository.DeleteBasketItem(username, item_to_delete));
        }

        /*[HttpPost]
        [ProducesResponseType(typeof(BasketCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BasketCart>> UpdateBasket([FromBody] BasketCart cart)
        {
            var updatedBasket = await _repository.UpdateBasket(cart);
            return Ok(updatedBasket);
        }
        */

        [HttpPost]
        [ProducesResponseType(typeof(BasketCart), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Add_To_Basket_Item(string username, BasketCartItem item_to_add)
        {
            if (string.IsNullOrEmpty(username)) return BadRequest();
            var basket_that_receives = await _repository.AddBasketItem(username, item_to_add);
            if (basket_that_receives == null) return StatusCode(500, "basket could not receive items");
            return Ok(basket_that_receives);
        }

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType(typeof(BasketCart), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
        {
            var basket = await _repository.GetBasket(basketCheckout.Username);
            if (basket == null)
            {
                _logger.LogError("Basket could not be found for user: " + basketCheckout.Username);
                return BadRequest();
            }
            var basketRemoved = await _repository.DeleteBasket(basketCheckout.Username);

            if (!basketRemoved)
            {
                _logger.LogError("Basket could not be removed for user: " + basketCheckout.Username);
                return BadRequest();
            }

            var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
            eventMessage.RequestId = Guid.NewGuid();

            try
            {
                _eventBus.PublishBasketCheckout(EventBusConstants.BasketCheckoutQueue, eventMessage);
            }
            catch (Exception e)
            {
                _logger.LogError("Error: Message can not be published for user: " + basketCheckout.Username + ", " + e.Message);
                throw;
            }

            return Accepted();
        }
    }
}