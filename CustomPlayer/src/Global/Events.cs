
namespace CustomPlayerGlobal
{
    class PlayerChangedEvent
    {
        private static PlayerChangedEvent instance;
        private static object syncRoot = new System.Object();

        public delegate void isPlayerChanged();
        private event isPlayerChanged playerChanged;


        public static PlayerChangedEvent getInstance()
        {
            if (instance == null)
            {
                lock(syncRoot)
                {
                    if (instance == null)
                        instance = new PlayerChangedEvent();
                }
            }     
            return instance;
        }


        public static void AddHandler(isPlayerChanged handler)
        {
            getInstance().playerChanged += handler;
        }

        
        public static void playerHasBeenChanged()
        {
            getInstance().playerChanged?.Invoke();
        }
    }
}
