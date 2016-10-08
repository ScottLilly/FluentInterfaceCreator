using Engine.Models;
using Engine.Resources;

namespace Engine
{
    public static class Actions
    {
        public static MethodAction Instantiate = new MethodAction {ID = 1, Name = Literals.Instantiate};
        public static MethodAction Continue = new MethodAction { ID = 2, Name = Literals.Continue};
        public static MethodAction Execute = new MethodAction { ID = 3, Name = Literals.Execute };
    }
}
