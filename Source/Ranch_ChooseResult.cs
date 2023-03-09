using System;
using System.Collections.Generic;
using System.Linq;
using AnimalBehaviours;
using Outposts;
using RimWorld;
using Verse;

namespace VOEE;

// TODO: Look at removing or actually incorporating this, it doesn't do anything now
// public class Ranch_ChooseResult : Outpost_ChooseResult
// {
//     [PostToSetings("Outposts.Settings.BodySize", PostToSetingsAttribute.DrawMode.Percentage, 1f, 0.01f, 2f)]
//     public float BodySize = 1f;
//
//     [PostToSetings("Outposts.Settings.Count", PostToSetingsAttribute.DrawMode.Percentage, 1f, 0.01f, 5f)]
//     public float CountMultiplier = 1f;
//
//     [PostToSetings("Outposts.Settings.Egg", PostToSetingsAttribute.DrawMode.Percentage, 0.5f, 0.01f, 2f)]
//     public float Egg = 0.5f;
//
//     [PostToSetings("Outposts.Settings.HungerRate", PostToSetingsAttribute.DrawMode.Percentage, 1f, 0.01f, 2f)]
//     public float HungerRate = 1f;
//
//     [PostToSetings("Outposts.Settings.Leather", PostToSetingsAttribute.DrawMode.Percentage, 0.5f, 0.01f, 2f)]
//     public float Leather = 0.5f;
//
//     [PostToSetings("Outposts.Settings.Meat", PostToSetingsAttribute.DrawMode.Percentage, 0.5f, 0.01f, 2f)]
//     public float Meat = 0.5f;
//
//     [PostToSetings("Outposts.Settings.Milk", PostToSetingsAttribute.DrawMode.Percentage, 0.5f, 0.01f, 2f)]
//     public float Milk = 0.5f;
//
//     [PostToSetings("Outposts.Settings.OtherProduct", PostToSetingsAttribute.DrawMode.Percentage, 0.5f, 0.01f, 2f)]
//     public float Other = 0.5f;
//
//     [PostToSetings("Outposts.Settings.Production", PostToSetingsAttribute.DrawMode.Percentage, 0.5f, 0.01f, 5f)]
//     public float ProductionMultiplier = 0.5f;
//
//     [PostToSetings("Outposts.Settings.Wool", PostToSetingsAttribute.DrawMode.Percentage, 0.5f, 0.01f, 2f)]
//     public float Wool = 0.5f;
//
//     public override List<ResultOption> ResultOptions
//     {
//         get
//         {
//             var resultOption = base.ResultOptions.FirstOrDefault();
//             if (resultOption?.Thing == null)
//             {
//                 return new List<ResultOption>();
//             }
//
//             var race = resultOption.Thing.race;
//             if (race == null)
//             {
//                 return new List<ResultOption>();
//             }
//
//             var outy = new List<ResultOption>
//             {
//                 new ResultOption
//                 {
//                     Thing = race.leatherDef ?? ThingDefOf.Leather_Plain,
//                     BaseAmount = Math.Max((int)(ProductionMultiplier * Leather *
//                                                 resultOption.Thing.GetStatValueAbstract(StatDefOf.LeatherAmount) *
//                                                 resultOption.BaseAmount *
//                                                 (resultOption.Thing.HasComp(typeof(CompEggLayer))
//                                                     ? resultOption.Thing.GetCompProperties<CompProperties_EggLayer>()
//                                                         .eggCountRange
//                                                         .Average
//                                                     : resultOption.Thing.race.litterSizeCurve == null
//                                                         ? 1
//                                                         : Rand.ByCurveAverage(resultOption.Thing.race
//                                                             .litterSizeCurve)) * 0.5 *
//                                                 15 /
//                                                 ((resultOption.Thing.race?.gestationPeriodDays ??
//                                                   (resultOption.Thing.GetCompProperties<CompProperties_EggLayer>() ==
//                                                    null
//                                                       ? 0
//                                                       : resultOption.Thing.GetCompProperties<CompProperties_EggLayer>()
//                                                           .eggFertilizedDef
//                                                           .GetCompProperties<CompProperties_Hatcher>()
//                                                           .hatcherDaystoHatch)) +
//                                                  (race.lifeStageAges.Last().minAge * 60))), 1)
//                 },
//                 new ResultOption
//                 {
//                     Thing = race.meatDef ?? ThingDefOf.Cow.race.meatDef ?? ThingDefOf.Meat_Human,
//                     BaseAmount = Math.Max((int)(ProductionMultiplier * Meat *
//                                                 resultOption.Thing.GetStatValueAbstract(StatDefOf.MeatAmount) *
//                                                 resultOption.BaseAmount *
//                                                 (resultOption.Thing.HasComp(typeof(CompEggLayer))
//                                                     ? resultOption.Thing.GetCompProperties<CompProperties_EggLayer>()
//                                                         .eggCountRange
//                                                         .Average
//                                                     : race.litterSizeCurve == null
//                                                         ? 1
//                                                         : Rand.ByCurveAverage(race.litterSizeCurve)) * 0.5 *
//                                                 15 /
//                                                 ((resultOption.Thing.race?.gestationPeriodDays ??
//                                                   (resultOption.Thing.GetCompProperties<CompProperties_EggLayer>() ==
//                                                    null
//                                                       ? 0
//                                                       : resultOption.Thing.GetCompProperties<CompProperties_EggLayer>()
//                                                           .eggFertilizedDef
//                                                           .GetCompProperties<CompProperties_Hatcher>()
//                                                           .hatcherDaystoHatch)) +
//                                                  (race.lifeStageAges.Last().minAge * 60))), 1)
//                 }
//             };
//             var milkies = resultOption.Thing.GetCompProperties<CompProperties_Milkable>();
//             if (milkies != null)
//             {
//                 outy.Add(
//                     new ResultOption
//                     {
//                         Thing = milkies.milkDef,
//                         BaseAmount = Math.Max((int)(ProductionMultiplier * Milk * resultOption.BaseAmount *
//                             (milkies.milkFemaleOnly ? 0.5 : 1) * milkies.milkAmount / milkies.milkIntervalDays * 15), 1)
//                     }
//                 );
//             }
//
//             var shearies = resultOption.Thing.GetCompProperties<CompProperties_Shearable>();
//             if (shearies != null)
//             {
//                 outy.Add(
//                     new ResultOption
//                     {
//                         Thing = shearies.woolDef,
//                         BaseAmount = Math.Max((int)(ProductionMultiplier * Wool * resultOption.BaseAmount *
//                             shearies.woolAmount /
//                             shearies.shearIntervalDays * 15), 1)
//                     }
//                 );
//             }
//
//             var eggies = resultOption.Thing.GetCompProperties<CompProperties_EggLayer>();
//             if (eggies is { eggProgressUnfertilizedMax: 1 })
//             {
//                 outy.Add(
//                     new ResultOption
//                     {
//                         Thing = eggies.eggUnfertilizedDef,
//                         BaseAmount = Math.Max((int)(ProductionMultiplier * Egg * resultOption.BaseAmount *
//                             (eggies.eggLayFemaleOnly ? 0.5 : 1) * eggies.eggCountRange.Average /
//                             eggies.eggLayIntervalDays * 15), 1)
//                     }
//                 );
//             }
//
//             var otheries = resultOption.Thing.GetCompProperties<CompProperties_AnimalProduct>();
//             if (otheries?.resourceDef != null)
//             {
//                 outy.Add(
//                     new ResultOption
//                     {
//                         Thing = otheries.resourceDef,
//                         BaseAmount = Math.Max((int)(ProductionMultiplier * Other * resultOption.BaseAmount *
//                             otheries.resourceAmount / otheries.gatheringIntervalDays * 15), 1)
//                     }
//                 );
//             }
//
//             return outy;
//         }
//     }
//
//     public override IEnumerable<ResultOption> GetExtraOptions()
//     {
//         var AnimalsSkillTotal = CapablePawns.ToList().Sum(p => p.skills.GetSkill(SkillDefOf.Animals).Level);
//         return from pkd in from pkd in DefDatabase<PawnKindDef>.AllDefs
//                 where pkd.race?.tradeTags != null && pkd.race.tradeTags.Contains("AnimalFarm") ||
//                       pkd.label == "boomalope"
//                 select pkd
//             select new ResultOption
//             {
//                 Thing = pkd.race,
//                 BaseAmount = Math.Max((int)Math.Ceiling(CountMultiplier /
//                                                         ((HungerRate * pkd.race.race.baseHungerRate) +
//                                                          (BodySize * pkd.race.race.baseBodySize)) *
//                                                         AnimalsSkillTotal), 1) //fuck rounding
//             };
//     }
// }
//


