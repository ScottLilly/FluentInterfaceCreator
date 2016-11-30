using Engine.Models;
using Engine.Resources;

namespace Engine
{
    public static class Actions
    {
        public static readonly MethodAction Instantiate = new MethodAction {ID = 1, Name = Literals.Instantiate};
        public static readonly MethodAction Continue = new MethodAction { ID = 2, Name = Literals.Continue};
        public static readonly MethodAction Execute = new MethodAction { ID = 3, Name = Literals.Execute };
    }
}
