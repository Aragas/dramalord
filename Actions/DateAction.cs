﻿using Dramalord.Conversations;
using Dramalord.Data;
using Dramalord.Extensions;
using Helpers;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Actions
{
    internal static class DateAction
    {
        internal static bool Apply(Hero hero, Hero target, List<Hero> closeHeroes)
        {
            HeroRelation heroRelation = hero.GetRelationTo(target); 

            int sympathy = hero.GetSympathyTo(target);
            int heroAttraction = hero.GetAttractionTo(target) / 10;
            int tagetAttraction = target.GetAttractionTo(hero) / 10;

            hero.GetDesires().Horny += heroAttraction;
            target.GetDesires().Horny += tagetAttraction;

            //heroRelation.Tension += (heroAttraction + tagetAttraction);
            heroRelation.Love += sympathy + ((heroAttraction + tagetAttraction) / 20);
            heroRelation.Trust += sympathy;
            heroRelation.LastInteraction = CampaignTime.Now.ToDays;

            if(hero == Hero.MainHero || target == Hero.MainHero)
            {
                Hero otherHero = (hero == Hero.MainHero) ? target : hero;
                TextObject banner = new TextObject("{=Dramalord069}You had a date with {HERO.LINK}. (Love {NUM}, Trust {NUM2})");
                StringHelpers.SetCharacterProperties("HERO", otherHero.CharacterObject, banner);
                MBTextManager.SetTextVariable("NUM", ConversationHelper.FormatNumber(sympathy + ((heroAttraction + tagetAttraction) / 20)));
                MBTextManager.SetTextVariable("NUM2", ConversationHelper.FormatNumber(sympathy));
                MBInformationManager.AddQuickInformation(banner, 0, otherHero.CharacterObject, "event:/ui/notification/relation");
            }

            int eventid = DramalordEvents.Instance.AddEvent(hero, target, EventType.Date, 3);

            Hero? witness = null;
            if (MBRandom.RandomInt(1, 100) < DramalordMCM.Instance?.ChanceGettingCaught)
            {
                witness = closeHeroes.GetRandomElementWithPredicate(h => h != hero && h != target);
                if (DramalordMCM.Instance.PlayerAlwaysWitness && hero != Hero.MainHero && target != Hero.MainHero && witness != Hero.MainHero && closeHeroes.Contains(Hero.MainHero))
                {
                    witness = Hero.MainHero;
                }
                if (witness != null)
                {
                    if (witness.IsEmotionalWith(hero) || witness.IsEmotionalWith(target))
                    {
                        DramalordIntentions.Instance.RemoveIntentionsTo(witness, hero);
                        DramalordIntentions.Instance.RemoveIntentionsTo(witness, target);
                        DramalordIntentions.Instance.AddIntention(witness, hero, IntentionType.Confrontation, eventid);
                        DramalordIntentions.Instance.AddIntention(witness, target, IntentionType.Confrontation, eventid);
                    }

                    if (witness == Hero.MainHero)
                    {
                        TextObject banner = new TextObject("{=Dramalord070}You saw {HERO.LINK} having a date with {TARGET.LINK}.");
                        StringHelpers.SetCharacterProperties("HERO", hero.CharacterObject, banner);
                        StringHelpers.SetCharacterProperties("TARGET", target.CharacterObject, banner);
                        MBInformationManager.AddQuickInformation(banner, 0, hero.CharacterObject, "event:/ui/notification/relation");
                    }
                    else if (hero == Hero.MainHero || target == Hero.MainHero)
                    {
                        Hero other = (hero == Hero.MainHero) ? target : hero;
                        TextObject banner = new TextObject("{=Dramalord071}{HERO.LINK} saw you having a date with {TARGET.LINK}.");
                        StringHelpers.SetCharacterProperties("HERO", witness.CharacterObject, banner);
                        StringHelpers.SetCharacterProperties("TARGET", other.CharacterObject, banner);
                        MBInformationManager.AddQuickInformation(banner, 0, witness.CharacterObject, "event:/ui/notification/relation");
                    }
                }
            }

            return true;
        }
    }
}
