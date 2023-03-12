using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using DoanApp.Commons;
using DoanApp.Models;
using DoanData.DoanContext;
using DoanData.Models;
using Microsoft.AspNetCore.Hosting;

namespace DoanApp.Services
{
    public class MessageService : IMessageService
    {
        private readonly DpContext _context;
        private readonly IWebHostEnvironment _enviroment;
        private string PathXML = "";
        public MessageService(DpContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _enviroment = environment;
            PathXML = _enviroment.WebRootPath + "\\Client\\fileXMLMessage\\xmlMessage.xml";

        }
        public async Task<int> Create(MessageRequest request, bool admin)
        {
            if (request != null)
            {

                var message = new Message();
                message.ReceiverId = request.ReceiverId;
                message.SenderId = request.SenderId;
                message.Watched = request.Watched;
                message.Content = request.Content;
                message.CreateDate = new GetDateNow().DateNow;
                if (admin)
                {
                    message.UserName = "Admin";
                    message.LoginExternal = false; ;
                    message.Avartar = "avartarDefault.jpg";
                }
                else
                {
                    message.LoginExternal = request.LoginExternal;
                    message.Avartar = request.Avartar;
                    message.UserName = request.UserName;
                }
                if (await CreateXMLMessage(message))
                {
                    return 1;
                }
                /* GetAll().Add(message);
                  return await _context.SaveChangesAsync();*/
            }
            return -1;
        }
        public int CheckIdMessage(string pathXML)
        {
            XmlDocument xd = new XmlDocument();
            xd.Load(pathXML);
            XmlNodeList nl = xd.GetElementsByTagName("Id");
            if (nl.Count > 0)
            {
                foreach (XmlNode node in nl.Item(nl.Count - 1))
                {
                    return int.Parse(node.InnerText);
                }
            }

            return 0;
        }

        public async Task<bool> CreateXMLMessage(Message message)
        {
            try
            {
                //set id initial if file nos exit in system
                message.Id = 1;

                //Check if not exit to create file xmlMessage.xml
                if (!File.Exists(PathXML))
                {
                    var doc = new XmlDocument();
                    //xml declaration is recommended, but not mandatory
                    var xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);

                    //create the root element
                    var root = doc.DocumentElement;
                    doc.InsertBefore(xmlDeclaration, root);
                    //string.Empty makes cleaner code
                    XmlElement mainBody = doc.CreateElement("Messages");
                    doc.AppendChild(mainBody);
                    XmlElement body = doc.CreateElement("message");
                    body.SetAttribute("id", message.Id.ToString());
                    mainBody.AppendChild(body);

                    //Element item
                    XmlElement id = doc.CreateElement("Id");

                    //XmlText text1 = doc.CreateTextNode(message.Id.ToString());
                    id.InnerText = message.Id.ToString();

                    XmlElement username = doc.CreateElement("UserName");
                    //XmlText text2 = doc.CreateTextNode(message.UserName);
                    username.InnerText = message.UserName;

                    XmlElement content = doc.CreateElement("Content");
                    content.InnerText = message.Content;
                    //XmlText text3 = doc.CreateTextNode(message.UserName);

                    XmlElement createDate = doc.CreateElement("CreateDate");
                    createDate.InnerText = message.CreateDate;

                    XmlElement avartar = doc.CreateElement("Avartar");
                    avartar.InnerText = message.Avartar;


                    XmlElement loginExternal = doc.CreateElement("LoginExternal");
                    loginExternal.InnerText = message.LoginExternal.ToString();

                    XmlElement checkWatched = doc.CreateElement("CheckWatched");
                    checkWatched.InnerText = message.CheckWatched.ToString();

                    XmlElement watched = doc.CreateElement("Watched");
                    watched.InnerText = message.Watched.ToString();

                    XmlElement senderId = doc.CreateElement("SenderId");
                    senderId.InnerText = message.SenderId.ToString();

                    XmlElement receiverId = doc.CreateElement("ReceiverId");
                    receiverId.InnerText = message.ReceiverId.ToString();
                    //End

                    body.AppendChild(id);
                    body.AppendChild(username);
                    body.AppendChild(content);
                    body.AppendChild(createDate);
                    body.AppendChild(avartar);
                    body.AppendChild(loginExternal);
                    body.AppendChild(checkWatched);
                    body.AppendChild(watched);
                    body.AppendChild(senderId);
                    body.AppendChild(receiverId);
                    doc.Save(PathXML);
                }
                else
                {
                    //Get id last xml in message
                    var id = CheckIdMessage(PathXML) + 1; ;
                    //end
                    var main = XElement.Load(PathXML);


                    var nodeMessage = new XElement("message");
                    nodeMessage.SetAttributeValue("id", id.ToString());

                    nodeMessage.Add(new XElement("Id", id.ToString()));
                    nodeMessage.Add(new XElement("UserName", message.UserName));
                    nodeMessage.Add(new XElement("Content", message.Content));
                    nodeMessage.Add(new XElement("CreateDate", message.CreateDate));
                    nodeMessage.Add(new XElement("Avartar", message.Avartar));
                    nodeMessage.Add(new XElement("LoginExternal", message.LoginExternal.ToString()));
                    nodeMessage.Add(new XElement("CheckWatched", message.CheckWatched.ToString()));
                    nodeMessage.Add(new XElement("Watched", message.Watched.ToString()));
                    nodeMessage.Add(new XElement("SenderId", message.SenderId.ToString()));
                    nodeMessage.Add(new XElement("ReceiverId", message.ReceiverId.ToString()));

                    //Save element 
                    main.Add(nodeMessage);
                    main.Save(PathXML);
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<int> Delete(int id)
        {
            if (id != 0)
            {
                var doc = XDocument.Load(PathXML);
                var listFind = FindElementById(id, doc);
                listFind.Result.ToList().ForEach(x => x.Remove());
                doc.Save(PathXML);
                return 1;
            }
            return -1;
        }
        public async Task<IEnumerable<XElement>> FindElementById(int id, XDocument doc)
        {
            var listFind = from node in doc.Descendants("Messages").Descendants("message")
                           let attr = node.Attribute("id")
                           where attr != null && attr.Value == id.ToString()
                           select node;
            return listFind;
        }

        public async Task<Message> FindMessageAsync(int id)
        {
            var mesage = GetAll().FirstOrDefault(x => x.Id == id);
            if (mesage != null) return mesage;
            return null;
        }

        public List<Message> GetAll()
        {
            var listMessage = new List<Message>();
            XmlDocument xd = new XmlDocument();

            if (File.Exists(PathXML))
            {
                xd.Load(PathXML);
                XmlNodeList nl = xd.GetElementsByTagName("message");

                //loop to delete messages longer than two weeks

                foreach (XmlNode item in nl)
                {
                    var timeMessage = DateTime.Parse(item.ChildNodes.Item(3).InnerText);
                    var caculator = DateTime.Now - timeMessage;
                    if (caculator.Days > 7)
                    {
                        var result = Delete(int.Parse(item.ChildNodes.Item(0).InnerText)).Result;
                        if (result > 0) continue;
                    }
                }
                //get node element into message
                foreach (XmlNode node in nl)
                {
                    var message = new Message();
                    message.Id = int.Parse(node.ChildNodes.Item(0).InnerText);
                    message.UserName = node.ChildNodes.Item(1).InnerText;
                    message.Content = node.ChildNodes.Item(2).InnerText;
                    message.CreateDate = node.ChildNodes.Item(3).InnerText;
                    message.Avartar = node.ChildNodes.Item(4).InnerText;
                    message.LoginExternal = bool.Parse(node.ChildNodes.Item(5).InnerText);
                    message.CheckWatched = bool.Parse(node.ChildNodes.Item(6).InnerText);
                    message.Watched = bool.Parse(node.ChildNodes.Item(7).InnerText);
                    message.SenderId = int.Parse(node.ChildNodes.Item(8).InnerText);
                    message.ReceiverId = int.Parse(node.ChildNodes.Item(9).InnerText);

                    listMessage.Add(message);
                }
            }

            return listMessage;
        }

        public List<Message> GetMessage_Partial(int senderId, int receiverId, bool flag = false)
        {
            var listMessage = GetAll().Where(x => x.SenderId == senderId && x.ReceiverId == receiverId ||
             x.SenderId == receiverId && x.ReceiverId == senderId).ToList();
            if (flag)
            {
                listMessage = listMessage.OrderByDescending(x => x.Id).Take(1).ToList();
            }
            return listMessage;
        }

        public List<Message_Vm> GetUserChat(int userId, List<AppUser> users)
        {
            var listGetUserChat = GetAll().Where(x => x.SenderId == userId).GroupBy(x => x.ReceiverId).
                Select(x => new { x.Key });
            var listUserChatRecei = GetAll().Where(x => x.ReceiverId == userId).GroupBy(x => x.SenderId).
                Select(x => new { x.Key });
            var listOverView = listGetUserChat.Union(listUserChatRecei).ToList();
            var listMessage = new List<Message>();
            var listNoneMessage = new List<Message>();
            var listMessage_Vm = new List<Message_Vm>();
            var listAll = GetAll().OrderByDescending(x => x.Id).ToList();
            var checkUserIntoSender = listAll.Any(X => X.SenderId == userId);
            if (checkUserIntoSender)
            {
                foreach (var j in listOverView)
                {
                    var flagExits = false;

                    foreach (var item in listAll)
                    {
                        var sender = item.ReceiverId;
                        var receiver = item.SenderId;

                        if (j.Key == item.SenderId && item.ReceiverId == userId || j.Key == item.ReceiverId && item.SenderId == userId)
                        {
                            var ms = new Message();
                            if (item.SenderId != userId)
                            {
                                item.SenderId = sender;
                                item.ReceiverId = receiver;

                                ms = item;
                                if (!item.Watched)
                                    ms.CheckWatched = true;
                            }
                            else
                                ms = GetUserInList(item);
                            flagExits = true;
                            if (listMessage.Count == 0)
                            {
                                listMessage.Add(ms);
                                break;
                            }
                            else
                            {

                                var checkItem = false;
                                if (item.SenderId != userId)
                                {
                                    checkItem = listMessage.Any(x => x.ReceiverId == item.ReceiverId);
                                }
                                else checkItem = listMessage.Any(x => x.SenderId == item.SenderId && x.ReceiverId == item.ReceiverId);
                                if (!checkItem)
                                {
                                    listMessage.Add(ms);
                                    break;
                                }

                            }
                        }

                    }

                    if (!flagExits)
                    {

                        foreach (var us in _context.AppUser.Where(x => x.Status))
                        {
                            var message = new Message();
                            if (j.Key == us.Id)
                            {
                                var checkUserAdmin = users.Any(X => X.Id == us.Id);
                                if (!checkUserAdmin)
                                {
                                    message.LoginExternal = false;
                                    message.Avartar = "avartarDefault.jpg";
                                    message.UserName = "Admin";
                                }
                                else
                                {
                                    message.LoginExternal = us.LoginExternal;
                                    message.Avartar = us.Avartar;
                                    message.UserName = us.FirtsName + " " + us.LastName;
                                }
                                message.SenderId = userId;
                                message.ReceiverId = j.Key;
                                foreach (var ms in listAll)
                                {
                                    if (j.Key == ms.ReceiverId)
                                    {
                                        message.Content = ms.Content;
                                        message.CreateDate = ms.CreateDate;
                                        break;
                                    }
                                }
                                if (listNoneMessage.Count == 0)
                                {
                                    listNoneMessage.Add(message);
                                }
                                else
                                {
                                    var checkItem2 = listNoneMessage.Contains(message);
                                    if (!checkItem2)
                                        listNoneMessage.Add(message);
                                }
                                break;
                            }

                        }
                    }

                }
            }
            else
            {
                foreach (var ms in listAll)
                {
                    if (userId == ms.ReceiverId)
                    {
                        foreach (var us2 in _context.AppUser.Where(x => x.Status))
                        {
                            var message = new Message();
                            if (ms.SenderId == us2.Id)
                            {

                                message.SenderId = userId;
                                message.ReceiverId = ms.SenderId;
                                message.Content = ms.Content;
                                message.CreateDate = ms.CreateDate;
                                if (ms.UserName == "Admin")
                                {
                                    message.Avartar = ms.Avartar;
                                    message.UserName = ms.UserName;
                                    message.LoginExternal = false;
                                }
                                else
                                {
                                    message.LoginExternal = us2.LoginExternal;
                                    message.Avartar = us2.Avartar;
                                    message.UserName = us2.FirtsName + " " + us2.LastName;
                                }
                                if (!ms.Watched) message.CheckWatched = true;
                                if (listNoneMessage.Count == 0)
                                    listNoneMessage.Add(message);
                                else
                                {
                                    var checknone = listNoneMessage.Any(X => X.ReceiverId == ms.SenderId);
                                    if (!checknone) listNoneMessage.Add(message);
                                }
                                break;
                            }


                        }

                    }

                }

            }
            listMessage.AddRange(listNoneMessage);
            foreach (var item in listMessage)
            {
                var ms_vm = new Message_Vm();
                var checkUserAdmin = users.Any(X => X.Id == item.ReceiverId);
                if (checkUserAdmin)
                {
                    ms_vm.LoginExternal = false;
                    ms_vm.Avartar = "avartarDefault.jpg";
                    ms_vm.UserName = "Admin";
                }
                else
                {
                    ms_vm.LoginExternal = item.LoginExternal;
                    ms_vm.Avartar = item.Avartar;
                    ms_vm.UserName = item.UserName;
                }
                ms_vm.Content = item.Content;
                ms_vm.CreateDate = item.CreateDate;
                ms_vm.ReceiverId = item.ReceiverId;
                ms_vm.SenderId = item.SenderId;
                ms_vm.Watched = item.Watched;
                ms_vm.CheckWatched = item.CheckWatched;
                foreach (var us in UserAuthenticated.ListtUser)
                {
                    if (us.Id == item.ReceiverId)
                    {
                        ms_vm.Online = true;
                        break;
                    }

                }
                listMessage_Vm.Add(ms_vm);
            }

            return listMessage_Vm.OrderByDescending(x => x.Id).ToList();
        }
        public Message GetUserInList(Message message)
        {
            foreach (var us2 in _context.AppUser.Where(x => x.Status))
            {
                if (us2.Id == message.ReceiverId)
                {
                    message.LoginExternal = us2.LoginExternal;
                    message.Avartar = us2.Avartar;
                    message.UserName = us2.FirtsName + " " + us2.LastName;
                    return message;
                }
            }
            return null;
        }

        public List<Message_Vm> GetUserNoneChat(List<Message_Vm> message_Vms, int userId, List<AppUser> users)
        {
            var listMessage_Vm = new List<Message_Vm>();
            var listUserFollow = _context.FollowChannel.Where(X => X.FromUserId == userId);
            var listUser = _context.AppUser.Where(x => x.Status && x.Id != userId && listUserFollow.Any(y => y.ToUserId == x.Id)).ToList();
            foreach (var item in users)
            {
                foreach (var item1 in listUser)
                {
                    if (item.UserName == item1.UserName)
                    {
                        listUser.Remove(item1);
                        break;
                    }
                }
            }
            foreach (var item in listUser)
            {
                var checkItem = message_Vms.Any(X => X.ReceiverId == item.Id);
                if (!checkItem)
                {
                    var ms_vm = new Message_Vm();
                    ms_vm.LoginExternal = item.LoginExternal;
                    ms_vm.Avartar = item.Avartar;
                    ms_vm.UserName = item.FirtsName + " " + item.LastName;
                    ms_vm.Content = "Trò chuyện ngay";
                    ms_vm.ReceiverId = item.Id;
                    ms_vm.SenderId = userId;
                    foreach (var us in UserAuthenticated.ListtUser)
                    {
                        if (item.Id == us.Id)
                        {
                            ms_vm.Online = true;
                            break;
                        }
                    }
                    listMessage_Vm.Add(ms_vm);
                }
            }
            return listMessage_Vm;
        }

        public async Task<int> Update(MessageRequest request)
        {
            var message = GetAll().FirstOrDefault(X => X.Id == request.Id);
            if (message != null)
            {
                message.Content = request.Content;
                _context.Update(message);
                return await _context.SaveChangesAsync();
            }
            return -1;
        }

        public async Task<int> UpdateAvartar(int userId, string img)
        {
            try
            {
                var doc = XDocument.Load(PathXML);
                var findListMessage = GetAll().Where(x => x.SenderId == userId);
                if (findListMessage.Count() > 0)
                {
                    foreach (var item in findListMessage)
                    {
                        FindElementById(item.Id, doc).Result.ToList().ForEach(x =>
                        {
                            x.Element("LoginExternal").Value = false.ToString();
                            x.Element("Avartar").Value = img;
                            doc.Save(PathXML);
                        });
                    }
                    return 1;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return 0;
        }

        public async Task<int> UpdateNameChannel(int userId, string username)
        {
            try
            {
                var doc = XDocument.Load(PathXML);
                var findListMessage = GetAll().Where(x => x.SenderId == userId);
                if (findListMessage.Count() > 0)
                {
                    foreach (var item in findListMessage)
                    {
                        FindElementById(item.Id, doc).Result.ToList().ForEach(x =>
                        {
                            x.Element("UserName").Value = username;
                            doc.Save(PathXML);
                        });
                    }
                    return 1;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return 0;
        }
        public async Task<int> UpdateWatched(int senderId, int receiverId, bool flag)
        {
            var countChange = 0;
            //Load file xml 
            var doc = XDocument.Load(PathXML);

            var listMessage = GetAll().ToList();
            if (receiverId == 0)
            {
                listMessage = listMessage.Where(x => x.ReceiverId == senderId && !x.Watched).ToList();
            }
            else
            {
                if (!flag)
                    listMessage = listMessage.Where(x => x.ReceiverId == senderId && !x.Watched).ToList();
                else
                    listMessage = listMessage.Where(x => x.SenderId == senderId && x.ReceiverId == receiverId && !x.Watched ||
                       x.SenderId == receiverId && x.ReceiverId == senderId && !x.Watched).ToList();
            }
            receiverId = receiverId == 0 ? senderId : receiverId;

            foreach (var item in listMessage)
            {
                if (item.ReceiverId == senderId)
                {
                    if (flag)
                    {
                        FindElementById(item.Id, doc).Result.ToList().ForEach(x =>
                        {
                            x.Element("Watched").Value = true.ToString();
                        });
                        doc.Save(PathXML);
                    }
                    countChange++;
                }

            }
            return countChange;
        }
    }
}
