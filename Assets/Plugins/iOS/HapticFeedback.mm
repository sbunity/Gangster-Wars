#import <UIKit/UIKit.h>
#import <CoreHaptics/CoreHaptics.h>

static const float kTransientMaxDuration = 0.05f;

static CHHapticEngine *_hapticEngine;
static id<CHHapticPatternPlayer> _hapticPlayer;
static NSUInteger _fallbackGeneration;

static BOOL GangsterWarsStartHapticEngine(void)
{
    if (!CHHapticEngine.capabilitiesForHardware.supportsHaptics)
        return NO;

    NSError *error = nil;
    if (_hapticEngine == nil)
    {
        _hapticEngine = [[CHHapticEngine alloc] initAndReturnError:&error];
        if (error != nil)
        {
            _hapticEngine = nil;
            return NO;
        }

        _hapticEngine.playsHapticsOnly = YES;
        _hapticEngine.autoShutdownEnabled = YES;
        _hapticEngine.resetHandler = ^{
            _hapticPlayer = nil;
            [_hapticEngine startAndReturnError:nil];
        };
    }

    [_hapticEngine startAndReturnError:&error];
    return error == nil;
}

static void GangsterWarsPlayFallbackPattern(const float *startTimes, const float *intensities, int count)
{
    NSUInteger generation = ++_fallbackGeneration;
    for (int i = 0; i < count; i++)
    {
        CGFloat intensity = intensities[i];
        dispatch_after(dispatch_time(DISPATCH_TIME_NOW, (int64_t)(startTimes[i] * NSEC_PER_SEC)), dispatch_get_main_queue(), ^{
            if (generation != _fallbackGeneration)
                return;

            UIImpactFeedbackGenerator *generator = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleMedium];
            [generator impactOccurredWithIntensity:intensity];
        });
    }
}

extern "C"
{
    void _GangsterWarsStopHaptics(void)
    {
        _fallbackGeneration++;
        if (_hapticPlayer != nil)
            [_hapticPlayer stopAtTime:CHHapticTimeImmediate error:nil];
        _hapticPlayer = nil;
    }

    void _GangsterWarsPlayHapticPattern(const float *startTimes, const float *durations, const float *intensities, const float *sharpnesses, int count)
    {
        if (count <= 0)
            return;

        if (!GangsterWarsStartHapticEngine())
        {
            GangsterWarsPlayFallbackPattern(startTimes, intensities, count);
            return;
        }

        NSMutableArray<CHHapticEvent *> *events = [NSMutableArray arrayWithCapacity:count];
        for (int i = 0; i < count; i++)
        {
            NSArray<CHHapticEventParameter *> *parameters = @[
                [[CHHapticEventParameter alloc] initWithParameterID:CHHapticEventParameterIDHapticIntensity value:intensities[i]],
                [[CHHapticEventParameter alloc] initWithParameterID:CHHapticEventParameterIDHapticSharpness value:sharpnesses[i]]
            ];

            CHHapticEvent *event = durations[i] <= kTransientMaxDuration
                ? [[CHHapticEvent alloc] initWithEventType:CHHapticEventTypeHapticTransient parameters:parameters relativeTime:startTimes[i]]
                : [[CHHapticEvent alloc] initWithEventType:CHHapticEventTypeHapticContinuous parameters:parameters relativeTime:startTimes[i] duration:durations[i]];
            [events addObject:event];
        }

        NSError *error = nil;
        CHHapticPattern *pattern = [[CHHapticPattern alloc] initWithEvents:events parameters:@[] error:&error];
        if (error != nil)
            return;

        _GangsterWarsStopHaptics();
        _hapticPlayer = [_hapticEngine createPlayerWithPattern:pattern error:&error];
        if (error != nil)
        {
            _hapticPlayer = nil;
            return;
        }

        [_hapticPlayer startAtTime:CHHapticTimeImmediate error:nil];
    }
}
