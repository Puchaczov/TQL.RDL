# TQL.RDL
Small language used to querying time for periodic task scheduling

# Short specification:

basic:<br/>
&emsp;repeat every ([second/minute/hour/day/month/year] | nth [seconds/minutes/hours/days/months/years])<br/>
optional:<br/>
&emsp;where left [operators:+,-,*,/,%,in,not in] right<br/>
optional:<br/>
&emsp;start at 'date constant'<br/>
&emsp;stop at 'date constant'<br/><br/>

left, right: [second/minute/hour/day/month/year/function()/constant]<br/>

Note: left and right <b>can contains subexpressions</b>.<br/>

# Default functions
&emsp;IsLastDayOfMonth(): bool<br/>
&emsp;IsLastDayOfMonth(DateTimeOffset time): bool<br/>
&emsp;IsDayOfWeek(DateTimeOffset time, long DayOfWeek): bool<br/>
&emsp;IsDayOfWeek(long DayOfWeek): bool<br/>
&emsp;IsWorkingDay(): bool<br/>
&emsp;IsEven(long number): bool<br/>
&emsp;IsOdd(long number): bool<br/><br/>
&emsp;GetDate(): DateTimeOffset<br/>
&emsp;Now(): DateTimeOffset<br/>
&emsp;UtcNow(): DateTimeOffset<br/><br/>
&emsp;GetDay(): long<br/>
&emsp;GetMonth(): long<br/>
&emsp;GetYear(): long<br/><br/>
&emsp;GetSecond(): long<br/>
&emsp;GetMinute(): long<br/>
&emsp;GetHour(): long<br/><br/>
&emsp;GetDayOfYear(): long

# How query can looks:

repeat every day where (IsLastDayOfMonth() and GetDay() in (monday,friday)) or (IsLastDayOfMonth() and GetMonth() % 2 = 0)

# C# methods bindings:

You can bind static methods by registering it in GlobalMetadata manager. Allowed return type of methods is DateTimeOffset, long and bool,
allowed parameters of functions are DateTimeOffset? and long?. Parameters count aren't limited.

# RDL virtual machine (Expression evaluator engine):

&emsp;+ Stack based (operate on two stacks: long? and DateTimeOffset?)<br/>
&emsp;+ Specialized small assembler with instructions: <br/>
&emsp;  GoTo, Break, Jump, CallExternal, CallExternal, Call, Ret, Push, Load, And, Or, GreaterOrEqual, <br/>
&emsp;  LessOrEqual, Greater, Less, Equal, NotEqual, In, NotIn, Modify, PrepareFunctionCall<br/>
&emsp;+ Less amount of instructions is better, specialized instructions with complex internals <br/>
&emsp;  are prefered over generate more set of instructions doing the same.<br/>
&emsp;+ Query hints support<br/>

# Flow

&emsp;String expression convertion to evaluator object uses such stages:<br/>
&emsp;"repeat where..." -> preprocessor -> lexer -> parser -> code generator -> RDL virtual machine.<br/><br/>

&emsp;Then, you can evaluate your next occurences.

# Notice

&emsp;<b>Not every features pointed in this document are already supported!</b>. It is very early stage currently.
