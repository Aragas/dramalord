﻿using Dramalord.Data;
using Dramalord.Extensions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Localization;

namespace Dramalord.Conversations
{
    internal static class PrisonerConversation
    {
        internal static void AddDialogs(CampaignGameStarter starter)
        {
            starter.AddPlayerLine("player_prisoner_start", "hero_main_options", "npc_prisoner_reply", "{=Dramalord273}Prisoner, I need to have a word with you.", ConditionPlayerCanApproach, null);

            starter.AddDialogLine("npc_prisoner_reply_yes", "npc_prisoner_reply", "player_prisoner_selection", "{=Dramalord274}It's not like I have much of a choice {TITLE}.", ConditionNpcAcceptsApproach, null);
            starter.AddDialogLine("npc_prisoner_reply_no", "npc_prisoner_reply", "close_window", "{=Dramalord275}I refuse to converse with you.", ConditionNpcDeclinesApproach, null);

            starter.AddPlayerLine("player_wants_prisonfun", "player_prisoner_selection", "npc_prisonfun_reaction", "{=Dramalord276}I would consider letting you go for... some special service in my bedroom.", null, null);
            starter.AddPlayerLine("player_wants_kill", "player_prisoner_selection", "npc_kill_reaction", "{=Dramalord277}It's time to end this. Your existence is bothering me.", null, null);
            starter.AddPlayerLine("player_wants_nothing", "player_prisoner_selection", "npc_end_conversation", "{=Dramalord255}Nevermind.", null, null);


            starter.AddDialogLine("npc_end_conversation", "npc_end_conversation", "hero_main_options", "{=Dramalord186}As you wish, {TITLE}.", ConditionEndConversation, null);

            starter.AddDialogLine("npc_prisonfun_reaction_yes", "npc_prisonfun_reaction", "close_window", "{=Dramalord278}You got yourself a deal! I think I will even enjoy it.", ConditionNpcAcceptsFun, ConsequenceNpcAcceptsFun);
            starter.AddDialogLine("npc_prisonfun_reaction_no", "npc_prisonfun_reaction", "player_prisoner_selection", "{=Dramalord279}Never! You will not taint my honor with such offers!", ConditionNpcDeclinesFun, null);

            starter.AddDialogLine("npc_kill_reaction_yes", "npc_kill_reaction", "close_window", "{=Dramalord280}Do what you must, you honorless swine!", ConditionNpcAcceptsKill, ConsequenceKillNpc);
            starter.AddDialogLine("npc_kill_reaction_no", "npc_kill_reaction", "close_window", "{=Dramalord281}Oh god! No please... I beg you!", ConditionNpcDeclinesKill, ConsequenceKillNpc);
            starter.AddDialogLine("npc_kill_reaction_offer", "npc_kill_reaction", "player_choose_pleasure", "{=Dramalord282}Wait {TITLE}! Why choose death if there's also pleasure?", ConditionNpcOffersKillAlternative, null);

            starter.AddPlayerLine("player_choose_pleasure_yes", "player_choose_pleasure", "close_window", "{=Dramalord283}Hmm... Alright. I accept. I spare you this time if you perform well.", null, ConsequenceNpcAcceptsFun);
            starter.AddPlayerLine("player_choose_pleasure_no", "player_choose_pleasure", "close_window", "{=Dramalord284}Well, your death is my sweetest pleasure.", null, ConsequenceKillNpc);
        }

        private static bool ConditionPlayerCanApproach()
        {
            if(Hero.OneToOneConversationHero.IsDramalordLegit() && Hero.OneToOneConversationHero.IsPrisoner)
            {
                if (Hero.OneToOneConversationHero.CurrentSettlement?.OwnerClan == Clan.PlayerClan || MobileParty.MainParty.PrisonRoster.Contains(Hero.OneToOneConversationHero.CharacterObject))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool ConditionNpcAcceptsApproach()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return Hero.OneToOneConversationHero.GetRelationWithPlayer() > -30;
        }

        private static bool ConditionNpcDeclinesApproach()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return Hero.OneToOneConversationHero.GetRelationWithPlayer() <= -30;
        }

        private static bool ConditionEndConversation()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return true;
        }

        private static bool ConditionNpcAcceptsFun()
        {
            return Hero.OneToOneConversationHero.GetHeroTraits().Honor < 0 && Hero.OneToOneConversationHero.GetPersonality().Openness > 0;
        }

        private static bool ConditionNpcDeclinesFun()
        {
            return !ConditionNpcAcceptsFun();
        }

        private static bool ConditionNpcAcceptsKill()
        {
            return Hero.OneToOneConversationHero.GetHeroTraits().Valor > 0;
        }

        private static bool ConditionNpcDeclinesKill()
        {
            return Hero.OneToOneConversationHero.GetHeroTraits().Honor > 0 && Hero.OneToOneConversationHero.GetHeroTraits().Valor <= 0;
        }

        private static bool ConditionNpcOffersKillAlternative()
        {
            return Hero.OneToOneConversationHero.GetHeroTraits().Honor <= 0 && Hero.OneToOneConversationHero.GetHeroTraits().Valor <= 0;
        }

        private static void ConsequenceNpcAcceptsFun()
        {
            ConversationHelper.ConversationEndedIntention = new HeroIntention(IntentionType.Intercourse, Hero.OneToOneConversationHero, -1);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
            EndCaptivityAction.ApplyByRansom(Hero.OneToOneConversationHero, Hero.MainHero);
        }

        private static void ConsequenceKillNpc()
        {
            ConversationHelper.ConversationEndedIntention = new HeroIntention(IntentionType.Execute, Hero.OneToOneConversationHero, -1);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }
    }
}
