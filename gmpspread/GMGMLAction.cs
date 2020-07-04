using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace gmpspread
{
    public class GMGMLAction : StreamBase
    {
        public enum EventName
        {
            EV_CREATE,
            EV_DESTROY,
            EV_ALARM,
            EV_STEP,
            EV_COLLISION,
            EV_KEYBOARD,
            EV_MOUSE,
            EV_OTHER,
            EV_DRAW,
            EV_KEYPRESS,
            EV_KEYRELEASE,
            EV_TRIGGER // not actually supported by GMPSP?
        }

        public enum ActionType
        {
            NORMAL,
            BEGIN,
            END,
            ELSE,
            EXIT,
            REPEAT,
            VARIABLE,
            CODE,

            // from LateralGM
            PLACEHOLDER,
            SEPARATOR,
            LABEL
        }

        public enum EventExecuteType
        {
            NONE,
            FUNCTION,
            CODE
        }

        public enum ArgTypes
        {
            CONSTANT = -1, // ???
            EXPRESSION,
            STRING,
            BOTH,
            BOOLEAN,
            MENU,
            SPRITE,
            SOUND,
            BACKGROUND,
            PATH,
            SCRIPT,
            GMOBJECT,
            ROOM,
            FONTR,
            COLOR,
            TIMELINE,
            FONTSTRING
        }

        public int LibID;
        public int ID;
        public ActionType Kind;
        public bool UseRelative;
        public bool IsQuestion;
        public bool UseApplyTo;
        public EventExecuteType ExeType;
        public GMString Name;
        public GMString Code;
        public int ArgumentCount;
        public int Who; // GMObject?
        public bool Relative;
        public bool IsNot;
        public int ArgTypeCount;
        public List<ArgTypes> ArgTypesList; // ???
        public List<GMString> Arguments; // actual arguments I guess. there was no bytecode so they're compiled @ runtime.

        public GMGMLAction(BinaryReader binaryReader)
        {
            LibID = binaryReader.ReadInt32();
            ID = binaryReader.ReadInt32();
            Kind = (ActionType)binaryReader.ReadInt32();
            UseRelative = ReadBool(binaryReader);
            IsQuestion = ReadBool(binaryReader);
            UseApplyTo = ReadBool(binaryReader);
            ExeType = (EventExecuteType)binaryReader.ReadInt32();
            Name = new GMString(binaryReader);
            Code = new GMString(binaryReader);
            ArgumentCount = binaryReader.ReadInt32();
            Who = binaryReader.ReadInt32();
            Relative = ReadBool(binaryReader);
            IsNot = ReadBool(binaryReader);
            ArgTypeCount = binaryReader.ReadInt32();
            ArgTypesList = new List<ArgTypes>(ArgTypeCount);
            Arguments = new List<GMString>(ArgTypeCount);
            //Debug.Assert(ArgumentCount == ArgTypeCount);
            for (int a = 0; a < ArgTypeCount; a++)
            {
                ArgTypesList.Add((ArgTypes)binaryReader.ReadInt32());
                Arguments.Add(new GMString(binaryReader));
            }
        }
    }
}
