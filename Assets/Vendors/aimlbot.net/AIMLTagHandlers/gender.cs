using System.Xml;
using AIMLbot.Normalize;
using AIMLbot.Utils;

namespace AIMLbot.AIMLTagHandlers
{
    /// <summary>
    ///     The atomic version of the gender element is a shortcut for:
    ///     <gender>
    ///         <star />
    ///     </gender>
    ///     The atomic gender does not have any content.
    ///     The non-atomic gender element instructs the AIML interpreter to:
    ///     1. replace male-gendered words in the result of processing the contents of the gender element
    ///     with the grammatically-corresponding female-gendered words; and
    ///     2. replace female-gendered words in the result of processing the contents of the gender element
    ///     with the grammatically-corresponding male-gendered words.
    ///     The definition of "grammatically-corresponding" is left up to the implementation.
    ///     Historically, implementations of gender have exclusively dealt with pronouns, likely due to the
    ///     fact that most AIML has been written in English. However, the decision about whether to
    ///     transform gender of other words is left up to the implementation.
    /// </summary>
    public class Gender : AIMLTagHandler
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
        public Gender(AIMLbot.Bot bot,
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
            if (templateNode.Name.ToLower() == "gender")
            {
                if (templateNode.InnerText.Length > 0)
                    // non atomic version of the node
                    return ApplySubstitutions.Substitute(bot, bot.GenderSubstitutions,
                        templateNode.InnerText);

                // atomic version of the node
                var starNode = GetNode("<star/>");
                var recursiveStar = new Star(bot, user, query, request, result, starNode);
                templateNode.InnerText = recursiveStar.Transform();
                if (templateNode.InnerText.Length > 0)
                    return ProcessChange();
                return string.Empty;
            }

            return string.Empty;
        }
    }
}