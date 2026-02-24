# RimClaw Actions

This is the current action surface exposed by `/actions`.

## Tasking / Work
- `set_priority` { pawn, work, level }
- `queue_build` { thing, pos[x,y,z] }

## Draft / Zone
- `draft` / `undraft` { pawn }
- `set_zone` { pawn, zone }

## Bills
- `create_bill` { table, recipe }
- `set_bill_count` { table, recipe, count }
- `set_bill_pause` { table, recipe, paused }
- `set_bill_repeat_mode` { table, recipe, repeat_mode: forever|count|target }
- `set_bill_target` { table, recipe, target_count }
- `set_bill_skill_range` { table, recipe, min_skill, max_skill }
- `set_bill_ingredients` { table, recipe, ingredients[] }

## Schedule
- `set_schedule` { pawn, schedule }  // 24 comma-separated values

## Combat
- `move_to` { pawn, target_pos[x,y,z] }
- `attack` { pawn, target }
- `attack_ranged` { pawn, target }
- `attack_pos` { pawn, target_pos[x,y,z] }
- `attack_thing` { pawn, target }
- `hold_position` { pawn }
- `flee` { pawn, target }
- `retreat` { pawn, target_pos[x,y,z] }
- `group_draft` / `group_undraft` { pawns[] }
- `group_move` { pawns[], target_pos[x,y,z] }
- `group_attack` { pawns[], target }
