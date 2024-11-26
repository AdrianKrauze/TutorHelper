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

            throw new ArgumentException("Nieobsługiwany przypadek e-maila.");
        }

        public string GetSubject(EmailCase emailCase)
        {
            return emailCase switch
            {
                EmailCase.Other => "Temat: Inne zapytanie",
                EmailCase.PageError => "Temat: Problem z stroną",
                EmailCase.ProblemWithLessons => "Temat: Problem z lekcjami",
                EmailCase.ProblemWithPayments => "Temat: Problem z płatnościami",
                EmailCase.ProblemWithStudents => "Temat: Problem z uczniami",
                EmailCase.Subscribe => "Temat: Subskrypcja",
                _ => "Temat: Inne"
            };
        }
    }
}