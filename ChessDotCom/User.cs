using System.Collections.ObjectModel;

namespace ChessDotCom
{
    public class User
    {
        public string Name { get; set; }
        public int Rating { get; set; }
        public int Id { get; set; }
    }

    public class UserCollection : KeyedCollection<int, User>
    {
        protected override int GetKeyForItem(User user)
        {
            return user.Id;
        }
    }
}
