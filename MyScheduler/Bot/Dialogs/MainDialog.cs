using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using MyGP2webapp.Controllers;
using MyGP2webapp.Data;
using MyGP2webapp.Models;
using System.Text;

namespace MyGP2webapp.Bot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly DBContextSystem _context;
        private readonly StudentController _studentController;

        public MainDialog(DBContextSystem context)
            : base(nameof(MainDialog))
        {
            _context = context;
            _studentController = new StudentController(_context);

            // Define the dialog steps
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                HiAsync,
                RegistrationOpenAsync,

                GetYearSemesterStepAsync,
                ConfirmYearSemesterStepAsync,
                ChooseYearStepAsync,
                ChooseSemesterStepAsync,

                ShowStudyPlanCoursesListStepAsync,
                ShowMajorElectiveCoursesListStepAsync,
                ShowElectiveCoursesListStepAsync,

                ChooseCoursesStepAsync,
                ProcessSelectedCoursesStepAsync,
                ScheduleProcessAsync,
                EndProcessAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> HiAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Hi, how are you today?") }, cancellationToken);
        }

        private async Task<DialogTurnResult> RegistrationOpenAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await Task.Delay(2000);
            if (!_studentController.IsOpen())
            {
                await SendMultipleMessagesAsync(stepContext.Context, cancellationToken,
                    "Looks like the registration is still closed",
                    "See you when the registration is opened",
                    "Bye");
                return await stepContext.EndDialogAsync();
            }

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("OK, let me check your data?") }, cancellationToken);
        }

        private async Task<DialogTurnResult> GetYearSemesterStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            _studentController.CalculateYearAndSemester();
            await Task.Delay(2000);

            if (StudentController.year == 132 && StudentController.semester == 132)
            {
                await SendMultipleMessagesAsync(stepContext.Context, cancellationToken,
                    "Congratulations! It looks like you're graduating.",
                    "You have finished all the courses in your plan. Bye!");
                return await stepContext.EndDialogAsync();
            }

            if (StudentController.year == 131 && StudentController.semester == 131)
            {
                await SendMultipleMessagesAsync(stepContext.Context, cancellationToken,
                    "Congratulations! Looks like you're close to graduating",
                    "You still have the Training, please contact the Training officer at your college",
                    "Bye");
                return await stepContext.EndDialogAsync();
            }

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmYearSemesterStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(ConfirmPrompt),
                new PromptOptions { Prompt = MessageFactory.Text($"It looks like you are in year {StudentController.year}, semester {StudentController.semester}. Is that correct?") },
                cancellationToken);
        }

        private async Task<DialogTurnResult> ChooseYearStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!(bool)stepContext.Result)
            {
                await stepContext.Context.SendActivityAsync("Oh! I'm sorry, please correct my information.");
                return await stepContext.PromptAsync(nameof(ChoicePrompt),
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text("Which year are you in?"),
                        Choices = ChoiceFactory.ToChoices(new List<string> { "1", "2", "3", "4" })
                    }, cancellationToken);
            }
            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> ChooseSemesterStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Which semester are you enrolling for?"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "1", "2", "3" })
            }, cancellationToken);
        }



        private async Task<DialogTurnResult> ShowStudyPlanCoursesListStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (int.TryParse((stepContext.Result as FoundChoice)?.Value, out int semester))
            {
                stepContext.Values["semester"] = semester;
            }
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Please wait...\nLet me check what courses you can take"), cancellationToken);
            await Task.Delay(2000);
            await _studentController.GetAvailableCoursesAsync();

            if (StudentController.studyPlanCourses.Count > 0)
            {
                int start = 0;
                StringBuilder courseInfoList = new StringBuilder();
                foreach (var c in StudentController.studyPlanCourses)
                {
                    start++;
                    var course = $"{start}.{c.CRS_A_NAME}";
                    courseInfoList.Append($"{course}" + Environment.NewLine);


                }
                var promptMessage = $"Major and University Requirements Courses:" + Environment.NewLine + courseInfoList;
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(promptMessage), cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Looks like you Finished Major and University Requirements Courses Hours"), cancellationToken);
            }
            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> ShowMajorElectiveCoursesListStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            if (StudentController.majorElectiveHours < 9 && StudentController.majorElectiveCourses.Count > 0)
            {
                    int start = StudentController.studyPlanCourses.Count;
                    StringBuilder courseInfoList = new StringBuilder();
                    foreach (var c in StudentController.majorElectiveCourses)
                    {
                        start++;
                        var course = $"{start}.{c.CRS_A_NAME}";
                        courseInfoList.Append($"{course}" + Environment.NewLine);


                    }
                    var promptMessage = $"Major Elective Courses:" + Environment.NewLine + courseInfoList;
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(promptMessage), cancellationToken);
                
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Looks like you Finished Major Elective Courses Hours"), cancellationToken);
            }

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> ShowElectiveCoursesListStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (StudentController.electiveHours < 9 && StudentController.electiveCourses.Count > 0)
            {
                int start = StudentController.studyPlanCourses.Count + StudentController.majorElectiveCourses.Count; 
                StringBuilder courseInfoList = new StringBuilder();
                foreach (var c in StudentController.electiveCourses)
                {
                    start++;
                    var course = $"{start}.{c.CRS_A_NAME}";
                    courseInfoList.Append($"{course}"+Environment.NewLine);
                     

                }
                var promptMessage = $"Elective Courses:" + Environment.NewLine + courseInfoList;
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(promptMessage), cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Looks like you Finished the Elective Courses Hours"), cancellationToken);
            }

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> ChooseCoursesStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptMessage = "Please enter the numbers of the courses you want to select from all the lists above, separated by commas like this 1,3,4 :";
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(promptMessage) }, cancellationToken);
        }

        private async Task<DialogTurnResult> ProcessSelectedCoursesStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userInput = (string)stepContext.Result;

            if (string.IsNullOrWhiteSpace(userInput))
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("No courses selected. Please try again"), cancellationToken);
                return await stepContext.ReplaceDialogAsync(nameof(MainDialog), null, cancellationToken);
            }

            var courses = StudentController.studyPlanCourses
                                           .Concat(StudentController.majorElectiveCourses)
                                           .Concat(StudentController.electiveCourses)
                                           .ToList();
            int maxCourseIndex = courses.Count;

            var selectedIndexes = userInput.Split(',')
                                           .Select(indexStr => int.TryParse(indexStr.Trim(), out int index) ? index : -1)
                                           .Where(index => index >= 1 && index <= maxCourseIndex)
                                           .Select(index => index - 1)
                                           .ToList();

            if (!selectedIndexes.Any())
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("No valid courses selected. Let's try again."), cancellationToken);
                return await stepContext.ReplaceDialogAsync(nameof(MainDialog), null, cancellationToken);
            }

            List<Course> selectedCourses = selectedIndexes.Select(index => courses[index]).ToList();
            int totalHours = selectedCourses.Sum(course => course.CRS_CR_HOURS);

            int semester = int.Parse(stepContext.Values["semester"].ToString());
            int maxHours = (semester == 1 || semester == 2) ? 18 : 10;
            int minHours = (semester == 1 || semester == 2) ? 12 : 6;

            if (totalHours > maxHours)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Sorry, but you can only enroll in a maximum of {maxHours} hours. You selected {totalHours} hours."), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Let's try again."), cancellationToken);
                return await stepContext.ReplaceDialogAsync(nameof(MainDialog), null, cancellationToken);
            }
            else if (totalHours < minHours)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Sorry, but you need to enroll in a minimum of {minHours} hours. You selected {totalHours} hours."), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Let's try again."), cancellationToken);
                return await stepContext.ReplaceDialogAsync(nameof(MainDialog), null, cancellationToken);
            }

            StudentController.coursesFor2 = selectedCourses;

            await _studentController.MakeScheduler();

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> ScheduleProcessAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (StudentController.ready)
            {
                return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
                {
                    Prompt = MessageFactory.Text("Your schedule is ready. Are you satisfied?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Yes", "No" })
                }, cancellationToken);
            }
            else
            {
                return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
                {
                    Prompt = MessageFactory.Text("Sorry, can't make a valid schedule from these courses. What would you like to do?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "End conversation", "Try again" })
                }, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> EndProcessAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var choiceResult = (FoundChoice)stepContext.Result;
            switch (choiceResult.Value)
            {
                case "End conversation":
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("See you again next semester. Bye!"), cancellationToken);
                    return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
                case "Try again":
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("let's try again."), cancellationToken);
                    return await stepContext.ReplaceDialogAsync(nameof(MainDialog),null, cancellationToken);
                case "Yes":
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("See you again next semester. Bye!"), cancellationToken);
                    return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
                case "No":
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("Oh! I'm sorry, let's try again."), cancellationToken);
                    return await stepContext.ReplaceDialogAsync(nameof(MainDialog),null, cancellationToken);
                default:
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("I'm sorry, I didn't understand that."), cancellationToken);
                    return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
        }

        private async Task SendMultipleMessagesAsync(ITurnContext turnContext, CancellationToken cancellationToken, params string[] messages)
        {
            foreach (var message in messages)
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(message), cancellationToken);
            }
        }
    }
}
