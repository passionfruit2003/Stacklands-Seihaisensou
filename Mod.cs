using HarmonyLib;
using System;
using System.Collections;
using UnityEngine;

namespace seihaisensouNS
{
    [HarmonyPatch(typeof(CreatePackLine), nameof(CreatePackLine.CreateBoosterBoxes))]
    public static class CreatePackLine_CreateBoosterBoxes_PATCH01
    {
        public static void Prefix(List<string> boosters)
        {
            boosters.Add("seihaisensou_booster_packs");
        }
    }
    public class seihaisensou : Mod
    {
        public override void Ready()
        {
            Logger.Log("Ready!");
            Harmony.PatchAll();
        }
    }
    // create a class called RedPanda which extends the Animal class
    public class RedPanda : Animal
    {
        // this method decides whether a card should stack onto this one
        protected override bool CanHaveCard(CardData otherCard)
        {
            if (otherCard.Id == "apple")
                return true; // if the other card is an apple, we will let it stack
            return base.CanHaveCard(otherCard); // otherwise, we will let Animal.CanHaveCard decide
        }

        // this method is called every frame, it is the CardData equivalent of the Update method
        public override void UpdateCard()
        {
            // the ChildrenMatchingPredicate method will return all child cards (cards stacked on the current one) that match a given predicate function
            // the given function checks if the card is an apple, so the apples variable will be a list of the apple cards on the red panda
            var apples = ChildrenMatchingPredicate(childCard => childCard.Id == "apple");
            if (apples.Count > 0) // if there are any apples on the red panda
            {
                int healed = 0; // create a variable to keep track of how much health the red panda gained
                foreach (CardData apple in apples) // for each apple on the red panda
                {
                    apple.MyGameCard.DestroyCard(); // destroy the apple card
                    HealthPoints += 2; // increase the red pandas health by 2
                    healed += 2; // keep track of how much it healed in total
                }
                AudioManager.me.PlaySound(AudioManager.me.Eat, Position); // play the eating sound at the red pandas position
                WorldManager.instance.CreateSmoke(Position); // create smoke particles at the red pandas position
                CreateHitText($"+{healed}", PrefabManager.instance.HealHitText); // create a heal text that displays how much it healed in total
            }
            base.UpdateCard(); // call the Animal.UpdateCard method
        }
    }
    public class magic_circle : CardData
    {
        public override bool CanHaveCardsWhileHasStatus()
        {
            if (IsBuilding)
            {
                return true;
            }
            return base.CanHaveCardsWhileHasStatus();
        }
        protected override bool CanHaveCard(CardData otherCard)
        {
            if (otherCard.MyCardType != CardType.Equipable && otherCard.MyCardType != CardType.Resources && otherCard.MyCardType != CardType.Humans && otherCard.MyCardType != CardType.Food && !(otherCard.Id == Id) && (otherCard.MyCardType != CardType.Structures || otherCard.IsBuilding))
            {
                return otherCard.MyCardType == CardType.Weather;
            }
            return true;
        }
    }
    public class ser : Villager
    {
        protected override bool CanHaveCard(CardData otherCard)
        {
            if (otherCard.Id == "seihaisensou_command_spell")
            return true; 
            return base.CanHaveCard(otherCard); 
        }
        public override void UpdateCard()
        {
            var apples = ChildrenMatchingPredicate(childCard => childCard.Id == "seihaisensou_command_spell");
            if (apples.Count > 0) 
            {
            int healed = 0; 
            foreach (CardData apple in apples)
            {
                apple.MyGameCard.DestroyCard(); 
                HealthPoints += 100;
                if (HealthPoints > BaseCombatStats.MaxHealth) HealthPoints = BaseCombatStats.MaxHealth;
                healed += 100;
            }
            AudioManager.me.PlaySound(AudioManager.me.Eat, Position); 
            WorldManager.instance.CreateSmoke(Position); 
            CreateHitText($"+{healed}", PrefabManager.instance.HealHitText); 
            }
            base.UpdateCard(); 
        }
    }
    public class holy_grail : CardData
    {
        protected override bool CanHaveCard(CardData otherCard)
        {
            return true;
        }
        public override void UpdateCard()
        {
            if (MyGameCard.HasChild)
            {
                MyGameCard.StartTimer(10f, CompleteMaking, SokLoc.Translate("seihaisensou_holy_grail_status"), GetActionId("CompleteMaking"));
            }
            else
            {
                MyGameCard.CancelTimer(GetActionId("CompleteMaking"));
            }
            base.UpdateCard();
        }

        public override bool CanHaveCardsWhileHasStatus()
        {
            return false;
        }

        [TimedAction("complete_making")]
        public void CompleteMaking()
        {
            CardData cardData = WorldManager.instance.CreateCard(base.transform.position, MyGameCard.Child.CardData.Id, faceUp: false, checkAddToStack: false);
            WorldManager.instance.StackSendCheckTarget(MyGameCard, cardData.MyGameCard, OutputDir, MyGameCard);
        }
    }
}