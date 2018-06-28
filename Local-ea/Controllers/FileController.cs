using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

using Project_2_Common.Models;
using Project_2_Common.Models.ChatviewModels;
using Project_2_Common.Models.MapViewModels;
using Project_2_Common.Data.Interfaces;
using Project_2_Common.Infrastructure.POCOs;
using Project_2_Common.Data.Repositories;
using Project_2_Common.Data;
using Project_2_Common.Models.ManageViewModels;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.FileProviders;
using System.IO;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Project_2_Common.Controllers
{
    public class FileController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly UserManager<ApplicationUser> _manager;
        private readonly IStoreMessages _store;

        public FileController(UserManager<ApplicationUser> manager, IStoreMessages store, IHostingEnvironment hostingEnvironment)
        {
            _store = store;
            _manager = manager;
            _hostingEnvironment = hostingEnvironment;
        }

        

        // GET: /File/Download
        [HttpPost]
        public async Task<IActionResult> Download(int? id)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(model);
            //}

            var user = await _manager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_manager.GetUserId(User)}'.");
            }

            var buddiesIDs = _store.myChatBuddies(user.Id);
            var buddiesAU = _manager.Users.Where(x => buddiesIDs.Contains(x.Id)).ToList();
            var buddies = new List<MapUserModel>();
            foreach (var u in buddiesAU)
            {
                buddies.Add(
                    new MapUserModel
                    {
                        Id = u.Id,
                        UserNameStr = u.UserNameStr,
                    });
            }

            List<string> txt = new List<string>();
            txt.Add("LIST OF MESSAGES");
            foreach (var u in buddies)
            {
                
                txt.Add(" ");
                txt.Add(" ");
                txt.Add("-->>" + u.UserNameStr + "<<--");
                txt.Add(" ");

                var messages = await _store.GetMessagesBetween(user.Id, u.Id);
                foreach (var item in messages)
                {
                    txt.Add("From: " + item.Sender.UserNameStr);
                    txt.Add("To: " + item.Receiver.UserNameStr);
                    txt.Add("Message: " + item.Body);
                    txt.Add("Date: " + item.DateSent.ToString("yyyy-dd-M--HH-mm-ss"));
                    txt.Add(" ");
                    //txt.Add("Read: " + item.Read);
                }
            }

            string str = string.Empty;
            IFileProvider fileProvider = _hostingEnvironment.ContentRootFileProvider;
            string fileName = user.UserNameStr + "_" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + "_" + "Messagess.txt";
            string content = (txt.Aggregate((i, j) => i + Environment.NewLine + j)).ToString();
            //return File(content, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

            MemoryStream memoryStream = new MemoryStream();
            TextWriter tw = new StreamWriter(memoryStream);

            tw.WriteLine(content);
            tw.Flush();
            tw.Close();

            return File(memoryStream.GetBuffer(), "text/plain", fileName);
        }
    }
}
