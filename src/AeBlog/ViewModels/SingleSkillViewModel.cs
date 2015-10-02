using AeBlog.Data;
using AeBlog.ViewModels;
using System.Collections.Generic;

public class SingleSkillViewModel : PortfolioViewModel
{
    public SingleSkillViewModel(IEnumerable<Portfolio> portfolios, string skill)
        : base (portfolios)
    {
        Skill = skill;
    }

    public string Skill { get; }
}