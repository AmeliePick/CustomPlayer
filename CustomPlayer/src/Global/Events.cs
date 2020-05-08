
namespace CustomPlayerGlobal
{
    abstract class Event
    {
        public delegate void EventPtr();
        



    }


    class PlayerChangedEvent : Event
    {
        protected static object syncRoot = new System.Object();
        protected static PlayerChangedEvent instance;

        protected event EventPtr playerChanged;

        public static PlayerChangedEvent getInstance()
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = new PlayerChangedEvent();
                }
            }
            return instance;
        }

        public static void AddHandler(EventPtr handler)
        {

            getInstance().playerChanged += handler;
        }


        public static void EventCall()
        {
            getInstance().playerChanged?.Invoke();
        }
    }


    class PlayerSavedEvent : Event
    {
        protected static object syncRoot = new System.Object();
        protected static PlayerSavedEvent instance;

        protected event EventPtr playerSaved;

        public static PlayerSavedEvent getInstance()
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = new PlayerSavedEvent();
                }
            }
            return instance;
        }

        
        public static void AddHandler(EventPtr handler)
        {

            getInstance().playerSaved += handler;
        }


        public static void EventCall()
        {
            getInstance().playerSaved?.Invoke();
        }
    }
}
