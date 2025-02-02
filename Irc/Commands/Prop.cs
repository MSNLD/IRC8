﻿using Irc.Enumerations;
using Irc.Objects;
using Irc.Props;
using Irc.Resources;

namespace Irc.Commands;

public class Prop : Command
{
    public Prop() : base(2, false)
    {
    }

    public override void Execute(ChatFrame chatFrame)
    {
        //chatFrame.User.Send(Raw.IRCX_ERR_NOTIMPLEMENTED(chatFrame.Server, chatFrame.User, nameof(Access)));
        // Passport hack
        //chatFrame.User.Name = "Sky";
        //chatFrame.User.GetAddress().Nickname = "Sky";
        //chatFrame.User.GetAddress().User = "A65F0CE7D05F0B4E";
        //chatFrame.User.GetAddress().Host = "GateKeeperPassport";
        //chatFrame.User.GetAddress().RealName = "Sky";
        //chatFrame.User.GetAddress().Server = "SERVER";
        //chatFrame.User.Register();
        // Register.Execute(chatFrame);

        // TODO: Resolve object first, e.g. IChatServer.GetObject(string)

        var objectName = chatFrame.Message.Parameters.First();
        if (!chatFrame.User.IsRegistered())
        {
            if (chatFrame.User.IsAuthenticated())
                // Prop $ NICK
                if (objectName == "$")
                    if (chatFrame.Message.Parameters.Count >= 2)
                    {
                        // TODO: This needs rewriting
                        if (string.Compare("NICK", chatFrame.Message.Parameters[1], true) == 0)
                        {
                            chatFrame.User.Nickname = chatFrame.User.Name;
                            SendProp(chatFrame.Server, chatFrame.User, chatFrame.User, "NICK",
                                chatFrame.User.Name);
                        }
                        else if (string.Compare("MSNREGCOOKIE", chatFrame.Message.Parameters[1], true) == 0)
                        {
                            if (chatFrame.Message.Parameters.Count >= 3)
                            {
                                var regcookie = chatFrame.Message.Parameters[2];
                                chatFrame.Server.ProcessCookie(chatFrame.User, "MSNREGCOOKIE",
                                    regcookie);
                            }
                        }
                        else if (string.Compare("SUBSCRIBERINFO", chatFrame.Message.Parameters[1], true) == 0)
                        {
                            var subscriberinfo = chatFrame.Message.Parameters[2];
                            chatFrame.Server.ProcessCookie(chatFrame.User, "SUBSCRIBERINFO",
                                subscriberinfo);
                        }
                        else if (string.Compare("MSNPROFILE", chatFrame.Message.Parameters[1], true) == 0)
                        {
                            // TODO: Hook up to actual prop
                            var msnprofile = chatFrame.Message.Parameters[2];
                            chatFrame.Server.ProcessCookie(chatFrame.User, "MSNPROFILE",
                                msnprofile);
                        }
                        else if (string.Compare("ROLE", chatFrame.Message.Parameters[1], true) == 0)
                        {
                            var role = chatFrame.Message.Parameters[2];
                            chatFrame.Server.ProcessCookie(chatFrame.User, "ROLE", role);
                        }
                        else
                        {
                            chatFrame.User.Send(Raws.IRCX_ERR_BADPROPERTY_905(chatFrame.Server, chatFrame.User,
                                chatFrame.Message.Parameters[1]));
                        }
                    }
            // PROP $ MSNREGCOOKIE
            // If regcookie is prop'd then no user is required, this fills in the USER info
            // Performs a NICK command
            // You have not authenticated or registered or whatever
        }
        else
        {
            ChatObject chatObject = null;

            // <$> The $ value is used to indicate the user that originated the request.
            if (objectName == "$")
                chatObject = chatFrame.User;
            else
                chatObject = chatFrame.Server.GetChatObject(objectName);

            if (chatObject == null)
            {
                // No such object
                chatFrame.User.Send(Raws.IRCX_ERR_NOSUCHOBJECT_924(chatFrame.Server, chatFrame.User, objectName));
            }
            else
            {
                if (chatFrame.Message.Parameters.Count >= 3)
                {
                    var propName = chatFrame.Message.Parameters[1].ToUpper();
                    var propValue = chatFrame.Message.Parameters[2];

                    // Setter
                    // TODO: Needs refactoring
                    var prop = ChannelPropCollection.PropRules[propName];
                    if (prop != null)
                    {
                        if (chatObject.CanBeModifiedBy(chatFrame.User))
                        {
                            var ircError = prop.EvaluateSet(chatFrame.User, chatObject, propValue);
                            if (ircError == EnumIrcError.ERR_NOPERMS)
                            {
                                chatFrame.User.Send(Raws.IRCX_ERR_NOACCESS_913(chatFrame.Server, chatFrame.User,
                                    chatObject));
                                return;
                            }

                            if (ircError == EnumIrcError.ERR_BADVALUE)
                            {
                                chatFrame.User.Send(Raws.IRCX_ERR_BADVALUE_906(chatFrame.Server, chatFrame.User,
                                    propValue));
                                return;
                            }

                            if (ircError == EnumIrcError.OK)
                            {
                                chatObject.Props[prop.Name] = propValue;
                                // prop.SetValue(propValue);
                                chatObject.Send(
                                    Raws.RPL_PROP_IRCX(chatFrame.Server, chatFrame.User, chatObject,
                                        prop.Name, propValue), prop.WriteAccessLevel);
                            }
                        }
                        else
                        {
                            chatFrame.User.Send(Raws.IRCX_ERR_NOACCESS_913(chatFrame.Server, chatFrame.User,
                                chatObject));
                        }
                    }
                    else
                    {
                        // Bad prop
                        chatFrame.User.Send(Raws.IRCX_ERR_BADPROPERTY_905(chatFrame.Server, chatFrame.User, objectName));
                    }
                }
                else
                {
                    // Getter
                    var props = new Dictionary<string?, string?>();
                    if (chatFrame.Message.Parameters[1] == "*")
                    {
                        props = chatObject.Props;
                    }
                    else
                    {
                        var propName = chatFrame.Message.Parameters[1];
                        if (chatObject.Props.TryGetValue(propName, out var propValue))
                            props[propName] = propValue;
                        else
                            // Bad prop
                            chatFrame.User.Send(Raws.IRCX_ERR_BADPROPERTY_905(chatFrame.Server, chatFrame.User,
                                objectName));
                    }

                    if (props.Count > 0) SendProps(chatFrame.Server, chatFrame.User, chatObject, props);
                }
            }
        }
    }

    // TODO: Rewrite this code
    public void SendProps(Server server, Objects.User? user, ChatObject targetObject, Dictionary<string?, string?> props)
    {
        var propsSent = 0;
        foreach (var prop in props)
        {
            if (ChannelPropCollection.PropRules[prop.Key].EvaluateGet((ChatObject)user, targetObject) ==
                EnumIrcError.ERR_NOPERMS)
            {
                if (props.Count == 1) user.Send(Raws.IRCX_ERR_SECURITY_908(server, user));
                continue;
            }

            if (targetObject is Channel)
            {
                var kvp = user.Channels.FirstOrDefault(x => x.Key == targetObject);
                if (kvp.Value != null)
                {
                    var member = kvp.Value;
                    // var propValue = prop.GetValue(targetObject);
                    if (!string.IsNullOrEmpty(prop.Value))
                    {
                        SendProp(server, user, targetObject, prop.Key, prop.Value);
                        propsSent++;
                    }
                }
            }
            else
            {
                // TODO: See if the below still works
                SendProp(server, user, targetObject, prop.Key, prop.Value);
            }

            propsSent++;
        }

        if (propsSent > 0) user.Send(IrcxRaws.IRCX_RPL_PROPEND_819(server, user, targetObject));
    }

    public void SendProp(Server server, Objects.User? user, ChatObject targetObject, string? propName,
        string? propValue)
    {
        user.Send(IrcxRaws.IRCX_RPL_PROPLIST_818(server, user, targetObject, propName, propValue));
    }
}