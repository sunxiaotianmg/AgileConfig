﻿using System;
using System.Threading.Tasks;
using AgileConfig.Server.Apisite.Filters;
using AgileConfig.Server.Apisite.Models;
using AgileConfig.Server.Data.Entity;
using AgileConfig.Server.IService;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace AgileConfig.Server.Apisite.Controllers
{
    [ModelVaildate]
    public class ServerNodeController : Controller
    {
        private readonly IServerNodeService _serverNodeService;
        public ServerNodeController(IServerNodeService serverNodeService)
        {
            _serverNodeService = serverNodeService;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody]ServerNodeVM model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            var oldNode = await _serverNodeService.GetAsync(model.Address);
            if (oldNode != null)
            {

                return Json(new
                {
                    success = false,
                    message = "节点已存在，请重新输入。"
                });
            }

            var node = new ServerNode();
            node.Address = model.Address;
            node.Remark = model.Remark;
            node.Status = NodeStatus.Offline;
            node.CreateTime = DateTime.Now;

            var result = await _serverNodeService.AddAsync(node);

            return Json(new
            {
                success = result,
                message = !result ? "添加节点失败，请查看错误日志" : ""
            });
        }


        [HttpPost]
        public async Task<IActionResult> Delete([FromBody]ServerNodeVM model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            var node = await _serverNodeService.GetAsync(model.Address);
            if (node == null)
            {
                return Json(new
                {
                    success = false,
                    message = "未找到对应的节点。"
                });
            }

            var result = await _serverNodeService.DeleteAsync(node);

            return Json(new
            {
                success = result,
                message = !result ? "删除节点失败，请查看错误日志" : ""
            });
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var nodes = await _serverNodeService.GetAllNodesAsync();

            return Json(new
            {
                success = true,
                data = nodes.OrderBy(n => n.CreateTime)
            });
        }
    }
}