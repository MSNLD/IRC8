﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Ircx.Objects;
using CSharpTools;
using System.Reflection;

namespace Core.Ircx.Commands
{
    class LISTX : Command
    {

        public LISTX(CommandCode Code) : base(Code)
        {
            base.MinParamCount = 1;
            base.DataType = CommandDataType.None;
            base.RegistrationRequired = true;
            base.ForceFloodCheck = true;
        }

        public new COM_RESULT Execute(Frame Frame)
        {
            Server server = Frame.Server;
            Message message = Frame.Message;
            User user = Frame.User;

            List<Channel> Channels = new List<Channel>();
            if (message.Data.Count > 1)
            {
                //
                if (Channel.IsChannel(message.Data[1]))
                {
                    //process as list of channels
                    List<string> ChannelNames = CSharpTools.Tools.CSVToArray(message.Data[1]);
                    //LISTX [<query list>] <mask>
                    //<query list> One or more query terms separated by spaces or commas.
                    //<mask>         Sequence of characters that is used to select a matching channel name or topic. The character * and ? are used for wildcard searches.
                    //      <#Select channels with less than # members.
                    //      >#Select channels with more than # members.
                    //      C<#Select channels created less than # minutes ago.
                    //      C>#Select channels created greater than # minutes ago.
                    //      L=<mask>Select channels with language property matching the mask string.
                    //      N=<mask>Select channels with name matching the mask string.
                    //      R=0Select unregistered channels.
                    //      R=1Select registered channels.
                    //      S=<mask>Select channels with subject matching the mask string.
                    //      T<#Select channels with a topic changed less than # minutes ago.
                    //      T>#Select channels with a topic changed greater than # minutes ago.
                    //      T=<mask>Select channels that topic matches the mask string.
                    //      <query limit>Maximum number of channels to be returned.
                    //      <mask>Sequence of characters that is used to select a matching channel 
                    //      name or topic. The character * and ? are used for wildcard searches. The 
                    //      Directory Server recognize * and ? as plain text. All masks specified are 
                    //      assumed to have wildcards at the beginning and end. The Chat Servers do 
                    //      support wildcards in masks.
                    for (int i = 0; i < ChannelNames.Count; i++)
                    {
                        if (!Channel.IsChannel(ChannelNames[i]))
                        {
                            //<- :SERVER 900 Administrator LISTX :Bad command
                            user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_ERR_BADCOMMAND_900, Data: new string[] { message.Data[0] }));
                            return COM_RESULT.COM_SUCCESS;
                        }
                        else
                        {
                            Channel c = server.Channels.GetChannel(ChannelNames[i]);
                            if (c != null) { Channels.Add(c); }
                        }
                    }
                }
                else
                {
                    int iRegisteredFlag = 0; // 0 = Neutral, -1 = False, 1 = True (not trying to be like VB!!!)
                    int iMaxListEntries = int.MaxValue;
                    int iMinMembers = 0, iMaxMembers = int.MaxValue;
                    int iMinutesBefore = int.MaxValue, iMinutesAfter = 0;
                    int iMinutesBeforeTopic = int.MaxValue, iMinutesAfterTopic = 0;
                    string LanguageMask = Resources.Null;
                    string NameMask = Resources.Null;
                    string SubjectMask = Resources.Null;
                    string TopicMask = Resources.Null;

                    bool bCheckMembers = false, bCheckCreation = false, bCheckLanguage = false, bCheckName = false,
                         bCheckRegistered = false, bCheckSubject = false, bCheckTopicChanged = false, bCheckTopic = false;

                    //process as list of commands
                    List<string> Params = CSharpTools.Tools.CSVToArray(message.Data[1], true);

                    for (int i = 0; i < Params.Count; i++)
                    {
                        bool bMatch = false;
                        if (Params[i].Length >= 2)
                        {
                            switch (Params[i][0])
                            {
                                case '<':
                                    {
                                        //      <#Select channels with less than # members.
                                        iMinMembers = CSharpTools.Tools.Str2Int(Params[i], 1);
                                        bCheckMembers = true;
                                        if (iMinMembers == -1) { /* Bad Command */ user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_ERR_BADCOMMAND_900, Data: new string[] { message.Data[0] })); return COM_RESULT.COM_SUCCESS; }
                                        break;
                                    }
                                case '>':
                                    {
                                        //      >#Select channels with more than # members.
                                        iMaxMembers = CSharpTools.Tools.Str2Int(Params[i], 1);
                                        bCheckMembers = true;
                                        if (iMaxMembers == -1) { /* Bad Command */ user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_ERR_BADCOMMAND_900, Data: new string[] { message.Data[0] })); return COM_RESULT.COM_SUCCESS; }
                                        break;
                                    }
                                case 'C':
                                    {
                                        //      C<#Select channels created less than # minutes ago.
                                        //      C>#Select channels created greater than # minutes ago.
                                        bCheckCreation = true;
                                        int iNumber = CSharpTools.Tools.Str2Int(Params[i], 2);
                                        if (iNumber == -1) { /* Bad Command */user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_ERR_BADCOMMAND_900, Data: new string[] { message.Data[0] })); return COM_RESULT.COM_SUCCESS; }
                                        else
                                        {
                                            if (Params[i][1] == (byte)'<')
                                            {
                                                iMinutesBefore = iNumber;
                                            }
                                            else if (Params[i][1] == (byte)'>')
                                            {
                                                iMinutesAfter = iNumber;
                                            }
                                            else { /* Bad Command */user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_ERR_BADCOMMAND_900, Data: new string[] { message.Data[0] })); return COM_RESULT.COM_SUCCESS; }
                                        }
                                        break;
                                    }
                                case 'L':
                                    {
                                        //      L=<mask>Select channels with language property matching the mask string.
                                        bCheckLanguage = true;
                                        if ((Params[i][1] == (byte)'=') && (Params[i].Length > 2))
                                        {
                                            LanguageMask = new string(Params[i].ToString().Substring(2).ToUpper());
                                        }
                                        else { /* bad command */ user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_ERR_BADCOMMAND_900, Data: new string[] { message.Data[0] })); return COM_RESULT.COM_SUCCESS; }
                                        break;
                                    }
                                case 'N':
                                    {
                                        //      N=<mask>Select channels with name matching the mask string.
                                        bCheckName = true;
                                        if ((Params[i][1] == (byte)'=') && (Params[i].Length > 2))
                                        {
                                            NameMask = new string(Params[i].ToString().Substring(2).ToUpper());
                                        }
                                        else { /* bad command */ user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_ERR_BADCOMMAND_900, Data: new string[] { message.Data[0] })); return COM_RESULT.COM_SUCCESS; }
                                        break;
                                    }
                                case 'R':
                                    {
                                        //      R=1Select registered channels.
                                        bCheckRegistered = true;
                                        if ((Params[i][1] == (byte)'=') && (Params[i].Length == 3))
                                        {
                                            if (Params[i][2] == (byte)'1') { iRegisteredFlag = 1; }
                                            else if (Params[i][2] == (byte)'0') { iRegisteredFlag = 0; }
                                            else { /* bad command */ user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_ERR_BADCOMMAND_900, Data: new string[] { message.Data[0] })); return COM_RESULT.COM_SUCCESS; }
                                        }
                                        else { /* bad command */ user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_ERR_BADCOMMAND_900, Data: new string[] { message.Data[0] })); return COM_RESULT.COM_SUCCESS; }
                                        break;
                                    }
                                case 'S':
                                    {
                                        //      S=<mask>Select channels with subject matching the mask string.
                                        bCheckSubject = true;
                                        if ((Params[i][1] == (byte)'=') && (Params[i].Length > 2))
                                        {
                                            SubjectMask = new string(Params[i].ToString().Substring(2).ToUpper());
                                        }
                                        else { /* bad command */ user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_ERR_BADCOMMAND_900, Data: new string[] { message.Data[0] })); return COM_RESULT.COM_SUCCESS; }
                                        break;
                                    }
                                case 'T':
                                    {
                                        //      T<#Select channels with a topic changed less than # minutes ago.
                                        //      T>#Select channels with a topic changed greater than # minutes ago.
                                        //      T=<mask>Select channels that topic matches the mask string.
                                        if (Params[i].Length > 2)
                                        {
                                            if (Params[i][1] == (byte)'<')
                                            {
                                                bCheckTopicChanged = true;
                                                int iNumber = CSharpTools.Tools.Str2Int(Params[i], 2);
                                                iMinutesBefore = iNumber;
                                            }
                                            else if (Params[i][1] == (byte)'>')
                                            {
                                                bCheckTopicChanged = true;
                                                int iNumber = CSharpTools.Tools.Str2Int(Params[i], 2);
                                                iMinutesAfter = iNumber;
                                            }
                                            else if (Params[i][1] == (byte)'=')
                                            {
                                                bCheckTopic = true;
                                                TopicMask = new string(Params[i].ToString().Substring(2).ToUpper());
                                            }
                                            else { /* Bad Command */ user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_ERR_BADCOMMAND_900, Data: new string[] { message.Data[0] })); return COM_RESULT.COM_SUCCESS; }
                                        }
                                        else { /* Bad Command */ user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_ERR_BADCOMMAND_900, Data: new string[] { message.Data[0] })); return COM_RESULT.COM_SUCCESS; }
                                        break;
                                    }
                                default:
                                    {
                                        if ((Params[i][0] >= 48) && (Params[i][0] <= 57))
                                        {
                                            //process as a number
                                            int maxListEntries = CSharpTools.Tools.Str2Int(Params[i]);
                                            if (maxListEntries == -1)
                                            {
                                                //<- :SERVER 900 Administrator LISTX :Bad command
                                                user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_ERR_BADCOMMAND_900, Data: new string[] { message.Data[0] }));
                                                return COM_RESULT.COM_SUCCESS;
                                            }
                                            else if (maxListEntries > 1) { iMaxListEntries = maxListEntries; }
                                            else { maxListEntries = int.MaxValue; }
                                        }
                                        else
                                        {
                                            //<- :SERVER 900 Administrator LISTX :Bad command
                                            user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_ERR_BADCOMMAND_900, Data: new string[] { message.Data[0] }));
                                            return COM_RESULT.COM_SUCCESS;
                                        }
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            if ((Params[i][0] >= 48) && (Params[i][0] <= 57))
                            {
                                //process as a number
                                int maxListEntries = CSharpTools.Tools.Str2Int(Params[i]);
                                if (maxListEntries == -1)
                                {
                                    //<- :SERVER 900 Administrator LISTX :Bad command
                                    user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_ERR_BADCOMMAND_900, Data: new string[] { message.Data[0] }));
                                    return COM_RESULT.COM_SUCCESS;
                                }
                                else if (maxListEntries > 1) { iMaxListEntries = maxListEntries; }
                                else { maxListEntries = int.MaxValue; }
                            }
                            else
                            {
                                //<- :SERVER 900 Administrator LISTX :Bad command
                                user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_ERR_BADCOMMAND_900, Data: new string[] { message.Data[0] }));
                                return COM_RESULT.COM_SUCCESS;
                            }
                        }
                    }

                    /* Find channel logic here */


                    for (int i = 0; i < server.Channels.Length; i++)
                    {
                        bool bFoundMembers = true, bFoundCreation = true, bFoundName = true, bFoundLanguage = true, bFoundRegistered = true, bFoundTopic = true, bFoundTopicChanged = true, bFoundSubject = true;
                        if (bCheckMembers)
                        {
                            if ((server.Channels[i].MemberList.Count <= iMinMembers) && (server.Channels[i].MemberList.Count >= iMaxMembers)) { bFoundMembers = true; }
                            else { bFoundMembers = false; }
                        }

                        if (bCheckCreation)
                        {
                            long iTime = ((DateTime.UtcNow.Ticks - Resources.epoch) / TimeSpan.TicksPerSecond), iAge = (iTime - server.Channels[i].Properties.CreationDate);
                            iAge = iAge / 60;
                            if ((iAge <= iMinutesBefore) && (iAge >= iMinutesAfter)) { bFoundCreation = true; }
                            else { bFoundCreation = false; }
                        }
                        if (bCheckName)
                        {
                            if (StringBuilderRegEx.EvaluateString(NameMask.ToString(), server.Channels[i].UCaseName.ToString(), true)) { bFoundName = true; }
                            else { bFoundName = false; }
                        }
                        if (bCheckLanguage)
                        {
                            if (StringBuilderRegEx.EvaluateString(LanguageMask.ToString(), server.Channels[i].Properties.Language.Value.ToString(), true)) { bFoundLanguage = true; }
                            else { bFoundLanguage = false; }
                        }
                        if (bCheckRegistered)
                        {
                            if (iRegisteredFlag == server.Channels[i].Modes.Registered.Value) { bFoundRegistered = true; }
                            else { bFoundRegistered = false; }
                        }
                        if (bCheckSubject)
                        {
                            if (StringBuilderRegEx.EvaluateString(SubjectMask.ToString(), server.Channels[i].Properties.Subject.Value.ToString(), true)) { bFoundSubject = true; }
                            else { bFoundSubject = false; }
                        }
                        if (bCheckTopic)
                        {
                            if (StringBuilderRegEx.EvaluateString(TopicMask.ToString(), server.Channels[i].Properties.Topic.Value.ToString(), true)) { bFoundTopic = true; }
                            else { bFoundTopic = false; }
                        }
                        if (bCheckTopicChanged)
                        {
                            long iTime = ((DateTime.UtcNow.Ticks - Resources.epoch) / TimeSpan.TicksPerSecond), iAge = (iTime - server.Channels[i].Properties.TopicLastChanged);
                            if ((iTime < iMinutesBeforeTopic) && (iTime > iMinutesAfterTopic)) { bFoundTopicChanged = true; }
                            else { bFoundTopicChanged = false; }
                        }

                        if (bFoundMembers && bFoundCreation && bFoundName && bFoundLanguage && bFoundRegistered && bFoundSubject && bFoundTopic && bFoundTopicChanged)
                        {
                            Channels.Add(server.Channels[i]);
                        }
                    }

                }
            }
            else { Channels = server.Channels.ObjectCollection.Cast<Channel>().ToList<Channel>(); }


            user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_RPL_LISTXSTART_811));

            for (int i = 0; i < Channels.Count; i++)
            {
                user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_RPL_LISTXLIST_812, Data: new string[] { Channels[i].Name, Channels[i].Modes.ChannelModeShortString, Channels[i].Properties.Topic.Value }, IData: new int[] { Channels[i].MemberList.Count, Channels[i].Modes.UserLimit.Value }));

            }
            user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_RPL_LISTXEND_817));
            return COM_RESULT.COM_SUCCESS;
        }
    }
}
