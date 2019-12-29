using AutoMapper;
using AutoMapperDemo.Models;
using AutoMapperDemo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace AutoMapperDemo.Controllers
{
    /// <summary>
    /// 用户控制器
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    [ApiVersion("1.0", Deprecated = false)]
    [Route("api/v{api-version:apiVersion}/[controller]/[action]")]
    public class UserController : Controller
    {
        private readonly IMapper _mapper;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        public UserController(IMapper mapper)
        {
            _mapper = mapper;
        }

        //public IActionResult Index()
        //{
        //    // Populate the user details from DB
        //    var user = GetUserDetails();

        //    //UserViewModel userViewModel = _mapper.Map<User,UserViewModel>(user);
        //    UserViewModel userViewModel = _mapper.Map<UserViewModel>(user);

        //    return View(userViewModel);
        //}


        /// <summary>
        /// 查询订单
        /// </summary>
        /// <param name="orderCode">订单代码</param>
        /// <param name="orderName">订单名称</param>
        /// <returns></returns>
        [HttpGet("{orderCode}/{orderName}")]
        [ApiVersion("1.0", Deprecated = false)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult QueryOrder(string orderCode, string orderName)
        {
            //OrderModel model = new OrderModel
            //{

            //    OrderCode = orderCode

            //};
            return Ok(new OrderModel
            {

                OrderCode = orderCode,
                OrderType = OrderTypeInfo.StoreEntry

            });
        }

        [HttpGet("GetBy2Params")] // 前台请求的方法名，可以与函数名不同
        public OkObjectResult Get(string param1, string param2)
        {

            return Ok(new
            {
                param1,
                param2
            });
        }


        /// <summary>
        /// 提交订单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiVersion("2.0", Deprecated = false)]
        public OrderModel SubOrder([FromBody]OrderModel model)
        {
            return model;
        }

        private static User GetUserDetails()
        {
            return new User()
            {
                Id = 1,
                FirstName = "John",
                LastName = "Smith",
                Email = "John.Smith@gmail.com",
                Address = new Address()
                {
                    Country = "US"
                }
            };
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class OrderModel
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderCode { get; set; }

        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal OrderAmount { get; set; }

        /// <summary>
        /// 订单类型
        /// <Remark>
        /// 0 商家入驻
        /// 1 线下交易
        /// </Remark>
        /// </summary>
        public OrderTypeInfo OrderType { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public enum OrderTypeInfo
    {
        /// <summary>
        /// 商家入驻
        /// </summary>
        [Description("商家入驻")]
        StoreEntry = 0,
        /// <summary>
        /// 线下交易
        /// </summary>
        [Description("线下交易")]
        StoreTrade = 1,

    }
}