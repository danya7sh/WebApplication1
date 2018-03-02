using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using WebApplication1.Models;

namespace WebApplication1
{
    public class ChatHub : Hub
    {
        public async Task PhController(String methodName, Phone phone)
        {
            await this.Clients.All.InvokeAsync(methodName, phone);
        }
        public async Task PrController(String methodName, Product product)
        {
            await this.Clients.All.InvokeAsync(methodName, product);
        }
    }
}