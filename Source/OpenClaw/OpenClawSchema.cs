namespace OpenClaw
{
    public static class OpenClawSchema
    {
        public static string Schema = @"{
  ""version"": 1,
  ""actions"": [
    {""action"":""set_priority"",""desc"":""Set work priority for a pawn (1=highest,4=lowest)."",""required"":[""pawn"",""work"",""level""],""types"":{""pawn"":""string"",""work"":""string"",""level"":""int(1-4)""},""example"":{""pawn"":""Name"",""work"":""Construction"",""level"":1}},
    {""action"":""queue_build"",""desc"":""Queue a build blueprint at a map position."",""required"":[""thing"",""pos""],""types"":{""thing"":""string"",""pos"":""int[3]""},""example"":{""thing"":""Wall"",""pos"":[12,0,18]}},
    {""action"":""draft"",""desc"":""Draft a pawn for combat."",""required"":[""pawn""],""types"":{""pawn"":""string""},""example"":{""pawn"":""Name""}},
    {""action"":""undraft"",""desc"":""Undraft a pawn."",""required"":[""pawn""],""types"":{""pawn"":""string""},""example"":{""pawn"":""Name""}},
    {""action"":""set_zone"",""desc"":""Assign pawn to a zone/area."",""required"":[""pawn"",""zone""],""types"":{""pawn"":""string"",""zone"":""string""},""example"":{""pawn"":""Name"",""zone"":""Home""}},

    {""action"":""create_bill"",""desc"":""Create a bill at a worktable."",""required"":[""table"",""recipe""],""types"":{""table"":""string"",""recipe"":""string""},""example"":{""table"":""ElectricStove"",""recipe"":""CookMealSimple""}},
    {""action"":""set_bill_count"",""desc"":""Set bill repeat count."",""required"":[""table"",""recipe"",""count""],""types"":{""table"":""string"",""recipe"":""string"",""count"":""int""},""example"":{""table"":""ElectricStove"",""recipe"":""CookMealSimple"",""count"":10}},
    {""action"":""set_bill_pause"",""desc"":""Pause or resume a bill."",""required"":[""table"",""recipe"",""paused""],""types"":{""table"":""string"",""recipe"":""string"",""paused"":""bool""},""example"":{""table"":""ElectricStove"",""recipe"":""CookMealSimple"",""paused"":true}},
    {""action"":""set_bill_repeat_mode"",""desc"":""Set bill repeat mode: forever|count|target."",""required"":[""table"",""recipe"",""repeat_mode""],""types"":{""table"":""string"",""recipe"":""string"",""repeat_mode"":""string""},""example"":{""table"":""ElectricStove"",""recipe"":""CookMealSimple"",""repeat_mode"":""forever""}},
    {""action"":""set_bill_target"",""desc"":""Set bill target count."",""required"":[""table"",""recipe"",""target_count""],""types"":{""table"":""string"",""recipe"":""string"",""target_count"":""int""},""example"":{""table"":""ElectricStove"",""recipe"":""CookMealSimple"",""target_count"":20}},
    {""action"":""set_bill_skill_range"",""desc"":""Restrict skill range for bill."",""required"":[""table"",""recipe"",""min_skill"",""max_skill""],""types"":{""table"":""string"",""recipe"":""string"",""min_skill"":""int"",""max_skill"":""int""},""example"":{""table"":""ElectricStove"",""recipe"":""CookMealSimple"",""min_skill"":4,""max_skill"":12}},
    {""action"":""set_bill_ingredients"",""desc"":""Restrict ingredients for a bill."",""required"":[""table"",""recipe"",""ingredients""],""types"":{""table"":""string"",""recipe"":""string"",""ingredients"":""string[]""},""example"":{""table"":""ElectricStove"",""recipe"":""CookMealSimple"",""ingredients"":[""Meat"",""Rice""]}},

    {""action"":""set_schedule"",""desc"":""Set 24h schedule (comma-separated)."",""required"":[""pawn"",""schedule""],""types"":{""pawn"":""string"",""schedule"":""string(24 entries)""},""example"":{""pawn"":""Name"",""schedule"":""work,work,work,anything,anything,anything,anything,anything,anything,anything,anything,anything,anything,anything,anything,anything,anything,anything,anything,anything,sleep,sleep,sleep,sleep""}},

    {""action"":""move_to"",""desc"":""Move pawn to position (drafts if needed)."",""required"":[""pawn"",""target_pos""],""types"":{""pawn"":""string"",""target_pos"":""int[3]""},""example"":{""pawn"":""Name"",""target_pos"":[10,0,12]}},
    {""action"":""attack"",""desc"":""Melee attack a target pawn."",""required"":[""pawn"",""target""],""types"":{""pawn"":""string"",""target"":""string""},""example"":{""pawn"":""Name"",""target"":""Raider1""}},
    {""action"":""attack_ranged"",""desc"":""Ranged attack a target pawn."",""required"":[""pawn"",""target""],""types"":{""pawn"":""string"",""target"":""string""},""example"":{""pawn"":""Name"",""target"":""Raider1""}},
    {""action"":""attack_pos"",""desc"":""Ranged attack a target position."",""required"":[""pawn"",""target_pos""],""types"":{""pawn"":""string"",""target_pos"":""int[3]""},""example"":{""pawn"":""Name"",""target_pos"":[14,0,18]}},
    {""action"":""attack_thing"",""desc"":""Ranged attack a target thing."",""required"":[""pawn"",""target""],""types"":{""pawn"":""string"",""target"":""string""},""example"":{""pawn"":""Name"",""target"":""Turret""}},
    {""action"":""hold_position"",""desc"":""Hold position in combat."",""required"":[""pawn""],""types"":{""pawn"":""string""},""example"":{""pawn"":""Name""}},
    {""action"":""flee"",""desc"":""Flee from a target pawn."",""required"":[""pawn"",""target""],""types"":{""pawn"":""string"",""target"":""string""},""example"":{""pawn"":""Name"",""target"":""Raider1""}},
    {""action"":""retreat"",""desc"":""Retreat to a position."",""required"":[""pawn"",""target_pos""],""types"":{""pawn"":""string"",""target_pos"":""int[3]""},""example"":{""pawn"":""Name"",""target_pos"":[5,0,6]}},

    {""action"":""group_draft"",""desc"":""Draft multiple pawns."",""required"":[""pawns""],""types"":{""pawns"":""string[]""},""example"":{""pawns"":[""Name1"",""Name2""]}},
    {""action"":""group_undraft"",""desc"":""Undraft multiple pawns."",""required"":[""pawns""],""types"":{""pawns"":""string[]""},""example"":{""pawns"":[""Name1"",""Name2""]}},
    {""action"":""group_move"",""desc"":""Move multiple pawns to a position."",""required"":[""pawns"",""target_pos""],""types"":{""pawns"":""string[]"",""target_pos"":""int[3]""},""example"":{""pawns"":[""Name1"",""Name2""],""target_pos"":[10,0,12]}},
    {""action"":""group_attack"",""desc"":""Group melee attack target pawn."",""required"":[""pawns"",""target""],""types"":{""pawns"":""string[]"",""target"":""string""},""example"":{""pawns"":[""Name1"",""Name2""],""target"":""Raider1""}}
  ]
}";
    }
}
