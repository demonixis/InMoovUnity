using System;
using System.Xml;
using AIMLbot.Utils;

namespace AIMLbot.AIMLTagHandlers
{
    /// <summary>
    ///     The thatstar element tells the AIML interpreter that it should substitute the contents of a
    ///     wildcard from a pattern-side that element.
    ///     The thatstar element has an optional integer index attribute that indicates which wildcard
    ///     to use; the minimum acceptable value for the index is "1" (the first wildcard).
    ///     An AIML interpreter should raise an error if the index attribute of a star specifies a
    ///     wildcard that does not exist in the that element's pattern content. Not specifying the index
    ///     is the same as specifying an index of "1".
    ///     The thatstar element does not have any content.
    /// </summary>
    public class Thatstar : AIMLTagHandler
    {
        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="bot">The bot involved in this request</param>
        /// <param name="user">The user making the request</param>
        /// <param name="query">The query that originated this node</param>
        /// <param name="request">The request inputted into the system</param>
        /// <param name="result">The result to be passed to the user</param>
        /// <param name="templateNode">The node to be processed</param>
        public Thatstar(AIMLbot.Bot bot,
            User user,
            SubQuery query,
            Request request,
            Result result,
            XmlNode templateNode)
            : base(bot, user, query, request, result, templateNode)
        {
        }

        protected override string ProcessChange()
        {
            if (templateNode.Name.ToLower() == "thatstar")
            {
                if (templateNode.Attributes.Count == 0)
                {
                    if (query.ThatStar.Count > 0)
                        return query.ThatStar[0];
                    bot.WriteToLog(
                        "ERROR! An out of bounds index to thatstar was encountered when processing the input: " +
                        request.rawInput);
                }
                else if (templateNode.Attributes.Count == 1)
                {
                    if (templateNode.Attributes[0].Name.ToLower() == "index")
                        if (templateNode.Attributes[0].Value.Length > 0)
                            try
                            {
                                var result = Convert.ToInt32(templateNode.Attributes[0].Value.Trim());
                                if (query.ThatStar.Count > 0)
                                {
                                    if (result > 0)
                                        return query.ThatStar[result - 1];
                                    bot.WriteToLog("ERROR! An input tag with a bady formed index (" +
                                                   templateNode.Attributes[0].Value +
                                                   ") was encountered processing the input: " + request.rawInput);
                                }
                                else
                                {
                                    bot.WriteToLog(
                                        "ERROR! An out of bounds index to thatstar was encountered when processing the input: " +
                                        request.rawInput);
                                }
                            }
                            catch
                            {
                                bot.WriteToLog("ERROR! A thatstar tag with a bady formed index (" +
                                               templateNode.Attributes[0].Value +
                                               ") was encountered processing the input: " + request.rawInput);
                            }
                }
            }

            return string.Empty;
        }
    }
}