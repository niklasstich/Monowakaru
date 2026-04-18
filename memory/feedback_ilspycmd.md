---
name: ilspycmd usage convention
description: When using ilspycmd to decompile assemblies, always disassemble into .dil_cache/ in the project folder first
type: feedback
---

Always disassemble target assemblies into `.dil_cache/` in the project folder before reading decompiled output. Do not pipe ilspycmd output directly.

**Why:** User preference for caching disassembly output, avoids repeated slow decompilation.

**How to apply:** When decompiling any `.dll` for inspection, run `ilspycmd <dll> -o .dil_cache/<name>/` first, then read from the cache.