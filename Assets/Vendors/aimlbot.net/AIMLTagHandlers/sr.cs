using System.Xml;

namespace AIMLbot.AIMLTagHandlers
{
    /// <summary>
    /// The sr element is a shortcut for: 
    /// 
    /// <srai><star/></srai> 
    /// 
    /// The atomic sr does not have any content. 
    /// </summary>
    public class sr : Utils.AIMLTagHandler
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="bot">The bot involved in this request</param>
        /// <param name="user">The user making the request</param>
        /// <param name="query">The query that originated this node</param>
        /// <param name="request">The request inputted into the system</param>
        /// <param name="result">The result to be passed to the user</param>
        /// <param name="templateNode">The node to be processed</param>
        public sr(Bot bot,
            User user,
            Utils.SubQuery query,
            Request request,
            Result result,
            XmlNode templateNode)
            : base(bot, user, query, request, result, templateNode)
        {
        }

        protected override string ProcessChange()
        {
            if (templateNode.Name.ToLower() == "sr")
            {
                var starNode = GetNode("<star/>");
                var recursiveStar = new star(bot, user, query, request, result, starNode);
                var starContent = recursiveStar.Transform();

                var sraiNode = GetNode("<srai>" + starContent + "</srai>");
                var sraiHandler = new srai(bot, user, query, request, result, sraiNode);
                return sraiHandler.Transform();
            }

            return string.Empty;
        }
    }
}