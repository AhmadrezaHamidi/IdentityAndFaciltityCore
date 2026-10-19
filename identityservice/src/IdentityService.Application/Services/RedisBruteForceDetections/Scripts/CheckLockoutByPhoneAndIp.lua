-- KEYS[1] = mobile_attempts_key, KEYS[2] = ip_attempts_key, KEYS[3] = lock_key
-- ARGV[1] = max_attempts, ARGV[2] = lock_duration

local mob_count = redis.call("INCR", KEYS[1])
local ip_count = redis.call("INCR", KEYS[2])

if mob_count == 1 then redis.call("EXPIRE", KEYS[1], ARGV[2]) end
if ip_count == 1 then redis.call("EXPIRE", KEYS[2], ARGV[2]) end

if tonumber(mob_count) >= tonumber(ARGV[1]) or tonumber(ip_count) >= tonumber(ARGV[1]) then
    redis.call("SET", KEYS[3], "1", "EX", ARGV[2])
    return 0 -- LOCKED
end

return 1 -- ALLOWED
