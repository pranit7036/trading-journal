using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TradingJournal.Interfaces.Services;
using TradingJournal.Models.Dto;
using TradingJournal.Models.Entity;

namespace TradingJournal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TradesController : ControllerBase
    {
        private readonly ITradeService _tradeService;
        public TradesController(ITradeService tradeService)  
        {
            // add interfaces and DI in this
            _tradeService = tradeService;
        }

        [HttpPost]
        [Route("add")]
        public async Task<ActionResult<object>> AddTrades([FromBody] TradesDto inputTrade)
        {
            if (inputTrade == null) {

                return BadRequest("Data is emptys");
            }

            if (IsNullOrEmpty(inputTrade.Symbol) || IsNullOrEmpty(inputTrade.InstrumentType) || IsNullOrEmpty(inputTrade.TradeType))
            {
                return BadRequest("Data is invalid");
            }

            var result = await _tradeService.ValidateTrades(inputTrade);

            return Ok(result);

            //if (inputTrade.)

            //if ()
            //{
                // return data null with valid reponse 
            //}
            // take data from the body and use service to save the data

            // proper response with status code for saving the data
        }

        [HttpGet]
        [Route("getdata")]
        public async Task<ActionResult<object>> GetData ()
        {
            var result = await _tradeService.GetTrades();
            
            if (result == null) {
                return BadRequest("Empty Data");
            }

            return result;
        }


        private static bool IsNullOrEmpty(string value) 
        {
            if (String.IsNullOrEmpty(value))
            {
                return true;
            }

            return false;
        }

    }
}
