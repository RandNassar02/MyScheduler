
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using MyGP2webapp.Controllers;
using MyGP2webapp.Data;

namespace MyGP2webapp.Bot.Bots
{
    public class DialogAndWelcomeBot<T> : DialogBot<T>
        where T : Dialog
    {

        private readonly StudentController sc2;


        public DialogAndWelcomeBot(ConversationState conversationState, UserState userState, T dialog, ILogger<DialogBot<T>> logger, DBContextSystem context)
            : base(conversationState, userState, dialog, logger, context)
        {
            sc2 = new StudentController(context);
        }



        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await SendIntroCardAsync(turnContext, cancellationToken);
                    await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
                }
            }
        }


        private async Task SendIntroCardAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {

            var card = new HeroCard
            {
                Title = $"Welcome {StudentController.student.Name}",
                Text = "I'm an assistant chatbot to help you make your semester schedule",
                Images = new List<CardImage>() { new CardImage("https://myshedulechatbotlogo.blob.core.windows.net/myschedulelogo/MySchedulerLogo.png") },

            };

            var response = MessageFactory.Attachment(card.ToAttachment());
            await turnContext.SendActivityAsync(response, cancellationToken);
        }
    }
}
