namespace CitroenAPI.Controllers
{
    public class IsRunningInstance
    {
        private static bool isRunning=false;
        public void SetIsRunning()
        {
            if (isRunning == false)
            {
                isRunning = true;
            }
            else isRunning = true;
        }
        public bool getIsRunning() => isRunning;
    }
}
