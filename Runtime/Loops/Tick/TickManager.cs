using System;
using System.Collections.Generic;

namespace d4160.Tick
{
    public static class TickManager
    {
        private static Dictionary<int, Tick> _ticks = new Dictionary<int, Tick>();
        private static Dictionary<string, Tick> _spetialTicks = new Dictionary<string, Tick>();

        public static Tick RegisterObject(string label, ITickObject tickObj, float fixedDeltaTime) {
            return RegisterObject(label, tickObj, () => { 
                var newTick = new Tick();
                _spetialTicks.Add(label, newTick);

                newTick.FixedDeltaTime = fixedDeltaTime;
                newTick.AddTickObject(tickObj);

                return newTick;
            });
        } 

        public static Tick RegisterObject(string label, ITickObject tickObj, int timesBySecond) {

            return RegisterObject(label, tickObj, () => { 
                var newTick = new Tick(timesBySecond);
                _spetialTicks.Add(label, newTick);

                newTick.AddTickObject(tickObj);

                return newTick;
            });
        } 

        private static Tick RegisterObject(string label, ITickObject tickObj, Func<Tick> onCreateNew) {
            if(tickObj == null) {
                return null;
            } 

            if (_spetialTicks.ContainsKey(label)) {
                Tick tick = _spetialTicks[label];
                tick.AddTickObject(tickObj);

                return tick;
            }
            else {
                return onCreateNew();
            }
        } 

        public static void UnregisterObject(string label, ITickObject tickObj) {
            if(tickObj == null) {
                return;
            } 

            if (_spetialTicks.ContainsKey(label)) {
                _spetialTicks[label].RemoveTickObject(tickObj);
            }
        } 

        public static Tick RegisterObject(int timesBySecond, ITickObject tickObj) {
            if(tickObj == null) {
                return null;
            } 

            if (_ticks.ContainsKey(timesBySecond)) {
                Tick tick = _ticks[timesBySecond];
                tick.AddTickObject(tickObj);

                return tick;
            }
            else {
                var newTick = new Tick(timesBySecond);
                _ticks.Add(timesBySecond, newTick);
                newTick.AddTickObject(tickObj);

                return newTick;
            }
        } 

        public static void UnregisterObject(int timesBySecond, ITickObject tickObj) {
            if(tickObj == null) {
                return;
            } 

            if (_ticks.ContainsKey(timesBySecond)) {
                _ticks[timesBySecond].RemoveTickObject(tickObj);
            }
        } 
    }
}