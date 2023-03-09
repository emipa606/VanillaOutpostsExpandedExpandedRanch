using System;
using System.Collections.Generic;
using System.Linq;
using AnimalBehaviours;
using Outposts;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace VOEE; 

public class Outpost_Ranching : Outpost {
    
    public int MaxAnimals {
        get =>
            (int)Math.Ceiling(
                TotalSkill(SkillDefOf.Animals) * CountMultiplier /
                (HungerRate * animalRaised.race.baseHungerRate +
                    BodySize * animalRaised.race.baseBodySize)
            );
    }

    static float TimeFromConceptionTilAdultEach(ThingDef animalDef) {
        var race = animalDef.race;
        var eggLayer = animalDef.GetCompProperties<CompProperties_EggLayer>();
        var litterSize = eggLayer != null
            ? eggLayer.eggCountRange.Average
            : race.litterSizeCurve != null ? Rand.ByCurveAverage(race.litterSizeCurve) : 1f;
        var gestationDays = eggLayer != null 
            ? eggLayer.eggLayIntervalDays + eggLayer.eggFertilizedDef.GetCompProperties<CompProperties_Hatcher>().hatcherDaystoHatch
            : race.gestationPeriodDays;
        var adulthoodDays = race.lifeStageAges.Last().minAge * 60f;
        var result = (gestationDays + adulthoodDays) / litterSize;
        return result;
    }

    public override List<ResultOption> ResultOptions {
        get {
            var list = new List<ResultOption>();
            var num = Math.Ceiling(CurrentAnimals + ToRaise);
            if (animalRaised == null) {
                return list;
            }
            if (num > MaxAnimals) {
                if (animalRaised.GetStatValueAbstract(StatDefOf.LeatherAmount) > 0f) {
                    list.Add(
                        new ResultOption
                        {
                            Thing = (animalRaised.race.leatherDef ?? ThingDefOf.Leather_Plain),
                            BaseAmount = (int)(ProductionMultiplier * Leather *
                                animalRaised.GetStatValueAbstract(StatDefOf.LeatherAmount) *
                                (num - MaxAnimals))
                        }
                    );
                }
                if (animalRaised.GetStatValueAbstract(StatDefOf.MeatAmount) > 0f) {
                    list.Add(
                        new ResultOption
                        {
                            Thing = animalRaised.race.meatDef,
                            BaseAmount = (int)(ProductionMultiplier * Meat *
                                animalRaised.GetStatValueAbstract(StatDefOf.MeatAmount) *
                                (num - MaxAnimals))
                        }
                    );
                }
                if (animalRaised.butcherProducts != null) {
                    list.AddRange(
                        animalRaised.butcherProducts
                        .Select(thingDefCountClass => new ResultOption {
                            Thing = thingDefCountClass.thingDef,
                            BaseAmount = (int)(ProductionMultiplier * thingDefCountClass.count * (num - MaxAnimals))
                        })
                    );
                }
            }
            if (animalRaised.GetCompProperties<CompProperties_Milkable>() is { } propertiesMilkable) {
                list.Add(
                    new ResultOption
                    {
                        Thing = propertiesMilkable.milkDef,
                        BaseAmount = (int)(ProductionMultiplier * Milk * num *
                            (propertiesMilkable.milkFemaleOnly ? 0.5 : 1.0) * propertiesMilkable.milkAmount /
                            propertiesMilkable.milkIntervalDays * 15.0)
                    }
                );
            }
            if (animalRaised.GetCompProperties<CompProperties_Shearable>() is { } propertiesShearable) {
                list.Add(
                    new ResultOption
                    {
                        Thing = propertiesShearable.woolDef,
                        BaseAmount = (int)(ProductionMultiplier * Wool * num *
                            propertiesShearable.woolAmount / propertiesShearable.shearIntervalDays * 15f)
                    }
                );
            }
            if (animalRaised.GetCompProperties<CompProperties_EggLayer>() is { eggProgressUnfertilizedMax: 1f } propertiesEggLayer) {
                list.Add(
                    new ResultOption
                    {
                        Thing = propertiesEggLayer.eggUnfertilizedDef,
                        BaseAmount = (int)(ProductionMultiplier * Egg * num *
                            (propertiesEggLayer.eggLayFemaleOnly ? 0.5 : 1.0) *
                            propertiesEggLayer.eggCountRange.Average /
                            propertiesEggLayer.eggLayIntervalDays * 15.0)
                    }
                );
            }
            if (animalRaised.GetCompProperties<CompProperties_AnimalProduct>() is { resourceDef: { } } propertiesAnimalProduct) {
                list.Add(
                    new ResultOption
                    {
                        Thing = propertiesAnimalProduct.resourceDef,
                        BaseAmount = (int)(ProductionMultiplier * Other * num *
                            propertiesAnimalProduct.resourceAmount / propertiesAnimalProduct.gatheringIntervalDays *
                            15f)
                    }
                );
            }
            return list;
        }
    }

    // TODO: Refactor ToRaise as a property
    public override void Produce() {
        ToRaise = CurrentAnimals * 0.5f * TimeFromConceptionTilAdultEach(animalRaised) * 15f;
        base.Produce();
        CurrentAnimals = Math.Max(CurrentAnimals + ToRaise, MaxAnimals);
    }

    static IEnumerable<IGrouping<ThingDef, Pawn>> GetBreedingGroups(IEnumerable<Pawn> caravanAnimals) {
        var groupedLivestock = caravanAnimals.Where(
                p => p.RaceProps.hasGenders
                    && (
                        AllowNonGrazers
                        || p.RaceProps.Eats(FoodTypeFlags.Tree)
                        || p.RaceProps.Eats(FoodTypeFlags.Plant)
                    )
            )
            .GroupBy(p => p.def);
        var breedingGroups = groupedLivestock.Where(g => g.Select(p => p.gender).Distinct().Count() > 1);
        return breedingGroups;
    }

    static bool SpawnCheck(int _, IEnumerable<Pawn> humanPawns) {
        var caravan = humanPawns.FirstOrDefault().GetCaravan();
        var caravanAnimals = caravan.PawnsListForReading.Where(p => p.RaceProps.Animal);
        var breedingGroups = GetBreedingGroups(caravanAnimals);
        return breedingGroups.Any();
    }

    // TODO: Refactor to OutpostExtension, currently called with reflection by Outposts.dll
    public static string CanSpawnOnWith(int tile, List<Pawn> humanPawns) {
        return !SpawnCheck(tile, humanPawns) ? "VOEE.Ranch.Moses".Translate() : null;
    }

    // TODO: Refactor to OutpostExtension, currently called with reflection by Outposts.dll
    public static string RequirementString(int tile, List<Pawn> humanPawns) {
        return "VOEE.Ranch.Moses".Translate().Requirement(SpawnCheck(tile, humanPawns));
    }

    // TODO: Leave the animals in the AllPawns/occupants collection so you can get them back out
    public void Generate() {
        var breedingGroups = GetBreedingGroups(AllPawns.Where(p => p.RaceProps.Animal));
        animalRaised = breedingGroups.First().Key;
        CurrentAnimals = AllPawns.Count(p => p.def == animalRaised);
        ((List<Pawn>)AllPawns).RemoveAll(p => p.def == animalRaised);
        ToRaise = CurrentAnimals * 0.5f * TimeFromConceptionTilAdultEach(animalRaised) * 15f;
    }

    // TODO: Leave the animals in the AllPawns/occupants collection so you can get them back out
    public override void Tick() {
        if (animalRaised == null) {
            Generate();
        }
        if (AllPawns.Any(o => o.def == animalRaised)) {
            CurrentAnimals += (
                from o in AllPawns
                where o.def == animalRaised
                select o
            ).Count();
            ((List<Pawn>)AllPawns).RemoveAll(o => o.def == animalRaised);
        }
        base.Tick();
    }

    public override string ProductionString() {
        string text = "VOEE.Ranch.HerdSize".Translate((int)CurrentAnimals) + " " +
            animalRaised.race.AnyPawnKind.GetLabelPlural();
        text += "\n" + "VOEE.Ranch.MaxHerdSize".Translate(MaxAnimals);
        if (base.ProductionString().Any()) {
            text = text + "\n" + base.ProductionString();
        }
        return text;
    }

    public override void ExposeData() {
        base.ExposeData();
        Scribe_Defs.Look(ref animalRaised, "animalRaised");
        Scribe_Values.Look(ref CurrentAnimals, "CurrentAnimals");
    }
    
    // TODO: Refactor ToRaise as a property
    public override void PostAdd() {
        base.PostAdd();
        if (animalRaised != null) {
            ToRaise = CurrentAnimals * 0.5f * TimeFromConceptionTilAdultEach(animalRaised) * 15f;
        }
    }

    [PostToSetings("Outposts.Setting.AllowNonGrazers", PostToSetingsAttribute.DrawMode.Checkbox, false)]
    public static bool AllowNonGrazers;

    [PostToSetings("Outposts.Settings.BodySize", PostToSetingsAttribute.DrawMode.Percentage, 1f, 0.01f, 2f)]
    public float BodySize = 1f;

    [PostToSetings("Outposts.Settings.HungerRate", PostToSetingsAttribute.DrawMode.Percentage, 1f, 0.01f, 2f)]
    public float HungerRate = 1f;

    [PostToSetings("Outposts.Settings.Leather", PostToSetingsAttribute.DrawMode.Percentage, 0.5f, 0.01f, 2f)]
    public float Leather = 0.5f;

    [PostToSetings("Outposts.Settings.Meat", PostToSetingsAttribute.DrawMode.Percentage, 0.5f, 0.01f, 2f)]
    public float Meat = 0.5f;

    [PostToSetings("Outposts.Settings.Milk", PostToSetingsAttribute.DrawMode.Percentage, 0.5f, 0.01f, 2f)]
    public float Milk = 0.5f;

    [PostToSetings("Outposts.Settings.Wool", PostToSetingsAttribute.DrawMode.Percentage, 0.5f, 0.01f, 2f)]
    public float Wool = 0.5f;

    [PostToSetings("Outposts.Settings.Egg", PostToSetingsAttribute.DrawMode.Percentage, 0.5f, 0.01f, 2f)]
    public float Egg = 0.5f;

    [PostToSetings("Outposts.Settings.OtherProduct", PostToSetingsAttribute.DrawMode.Percentage, 0.5f, 0.01f, 2f)]
    public float Other = 0.5f;

    [PostToSetings("Outposts.Settings.Production", PostToSetingsAttribute.DrawMode.Percentage, 0.5f, 0.01f, 5f)]
    public float ProductionMultiplier = 0.5f;

    [PostToSetings("Outposts.Settings.Count", PostToSetingsAttribute.DrawMode.Percentage, 1f, 0.01f, 5f)]
    public float CountMultiplier = 1f;

    public ThingDef animalRaised;

    public float CurrentAnimals;

    // TODO: Refactor ToRaise as a property
    public float ToRaise;
}