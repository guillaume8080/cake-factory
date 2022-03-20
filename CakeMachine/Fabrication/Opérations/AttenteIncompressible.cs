namespace CakeMachine.Fabrication.Opérations
{
    internal static class AttenteIncompressible
    {
        public static void Attendre(TimeSpan howLong) => Thread.Sleep(howLong);
        // howlong ne définie pas un temps d'attente mais impose un temps d execution à la Task
       public static async Task AttendreAsync(TimeSpan howLong) => await Task.Delay(howLong);
        // public static async Task AttendreAsync(TimeSpan howLong) =>  Thread.Sleep(howLong);
    }
}
