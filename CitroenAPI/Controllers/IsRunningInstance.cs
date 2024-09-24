namespace CitroenAPI.Controllers
{
    public class IsRunningInstance
    {
        private static bool isRunning = false;
        public void SetIsRunning()
        {
            if (isRunning == false) 
                isRunning = true;
            else 
                isRunning = false;
        }
        public bool getIsRunning() => isRunning;
    }
}
