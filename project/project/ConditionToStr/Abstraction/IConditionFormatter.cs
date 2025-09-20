using project.Plugins.Pluginmodels;

namespace project.ConditionToStr.Abstraction
{
    public interface IConditionFormatter
    {
        public string Format(BaseCondition condition);
    }
}