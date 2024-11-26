using TutorHelper.EmailStrategy.Strategies;
namespace TutorHelper.EmailStrategy
{
    public class EmailStrategyFactory
    {
        private readonly Dictionary<EmailCase, IEmailStrategy> _strategies;

        public EmailStrategyFactory()
        {
            _strategies = new Dictionary<EmailCase, IEmailStrategy>
            {
                {EmailCase.Other, new OtherStrategy() },
                {EmailCase.PageError, new PageErrorStrategy() },
                {EmailCase.ProblemWithLessons, new ProblemWithLessonsStrategy() },
                {EmailCase.ProblemWithPayments, new ProblemWithPaymentsStrategy() },
                {EmailCase.ProblemWithStudents, new ProblemWithStudentsStrategy() },
                {EmailCase.Subscribe, new SubscribeStrategy() }

            };
        }

        public IEmailStrategy GetStrategy(EmailCase emailCase)
        {
            if (_strategies.TryGetValue(emailCase, out var strategy))
            {
                return strategy;
            }

            return new OtherStrategy(); 
        }


    }
}