using FluentScheduler;

namespace TeamSpeakNotifier
{
    public class TaskRegistry : Registry
    {
        public TaskRegistry(AppSettings settings)
        {
            var checker = new TeamSpeakUserChecker(settings);

            Schedule(() => checker).NonReentrant().ToRunNow().AndEvery(settings.JobInterval).Seconds();
        }
    }
}