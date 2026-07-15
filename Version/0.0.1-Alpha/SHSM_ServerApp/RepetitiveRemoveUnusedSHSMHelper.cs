using SHSM_ServerApp.Helper;

namespace SHSM_ServerApp
{
    public class RepetitiveRemoveUnusedSHSMHelper : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var delay = TimeSpan.FromSeconds(90);

            while (!stoppingToken.IsCancellationRequested)
            {
                //Console.WriteLine($"Worker running at: {DateTime.Now}");

                await DoWork();

                await Task.Delay(delay, stoppingToken);
            }
        }

        private Task DoWork()
        {
            String User_ID = "";
            int Index = 0;
            DateTime SHSMDateTime = DateTime.MinValue;
            DateTime CurrentDateTime = DateTime.UtcNow.AddHours(8);
            while(Index < SHSMOpsHelper.ListsOfRegisteredUsers.Count) 
            {
                User_ID = SHSMOpsHelper.ListsOfRegisteredUsers[Index].RegisteredUser.User_ID;
                SHSMDateTime = SHSMOpsHelper.ListsOfRegisteredUsers[Index].ValidDateTime;
                if (SHSMDateTime.CompareTo(DateTime.MinValue) != 0) 
                {
                    if (CurrentDateTime.CompareTo(SHSMDateTime) >= 0) 
                    {
                        RemoveSHSMHelper.WholeRemoveSHSM(Index, User_ID);
                    }
                }
                Index += 1;
            }
            return Task.CompletedTask;
        }
    }
}
