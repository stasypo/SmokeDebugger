# Debugger Smoke Testing checklist

# System requirements

| Requirement type | Details |
| --- | --- |
| OS | Windows x64 – the whole checklist <br />macOS arm64 – basics only |
| Technologies | ASP.NET Core – the whole checklist<br />Everything else – basics only<br />Blazor WASM, ASP.NET Classic, File-based app, ASP.NET Core and Angular, Desktop, Aspire, RemDev |
| Frameworks | .NET with the LTS/STS .NET SDK, .NET Framework 4.8, Mono, Multitarget |
| Languages | C#, C# code blocks in CSHTML/Razor, F#, Razor, JS/TS |

# Test solutions

This repository (`SmokeDebugger.sln`) contains the projects referenced by every item below.

Additional solutions: <https://github.com/stasypo/DebuggerRegressionTesting>

# Test projects

| Project | Framework / technology | Role in the checklist |
| --- | --- | --- |
| `ConsoleApp1` | .NET 10 console | **Scenario catalog.** Breakpoints, stepping, evaluation, visualizers, threads/tasks, exceptions, mixed mode, console output |
| `BlazorAppMultitarget` | ASP.NET Core Blazor Server, `net10.0` + `net9.0` | Whole checklist on ASP.NET Core; Multitarget framework switching; C# in Razor `@code` blocks |
| `BlazorAppWASM` | Blazor WebAssembly, `net10.0` | Basics under WASM |
| `WebAppWithAngular` | ASP.NET Core + Angular, `net7.0` | C# and JS/TS breakpoints in one session |
| `WebApplicationClassic` | ASP.NET Classic, .NET Framework 4.8 | Basics on .NET Framework; C# code blocks in CSHTML |
| `WpfApp1` | WPF desktop, `net10.0-windows` | Desktop basics; Attach/Detach; JIT debugger |
| `SingleFileApp1.cs` | File-based app | Long-running console app: Pause/Resume, Attach, console output |

Not represented in this solution: **Aspire**. Use a template AspireStarter App
solution.

## Technology and language pass

The **Basics** section below is executed in full against `ConsoleApp1` and `BlazorAppMultitarget`.
The "basics only" technologies and the remaining languages are covered by one short pass each:
start the app under the debugger, hit the scenario breakpoint, check Variables, step a few lines,
resume and stop.

| Technology / language | Where | Scenario name |
| --- | --- | --- |
| ASP.NET Core (whole checklist) | `BlazorAppMultitarget` | `multitarget-session` |
| Multitarget (`net10.0` / `net9.0`) | `BlazorAppMultitarget/Program.cs` | `multitarget-session` |
| C# in Razor `@code` | `BlazorAppMultitarget/Components/Pages/Counter.razor` | `razor-code-block-breakpoint` |
| Blazor WASM | `BlazorAppWASM/Pages/Counter.razor` | `wasm-basics` |
| ASP.NET Classic / .NET Framework 4.8 | `WebApplicationClassic/Controllers/HomeController.cs` | `aspnet-classic-basics` |
| C# code blocks in CSHTML | `WebApplicationClassic/Views/Home/About.cshtml` | `cshtml-code-block-breakpoint` |
| ASP.NET Core + Angular (C# side) | `WebAppWithAngular/Controllers/WeatherForecastController.cs` | `aspnet-and-angular-request` |
| JS/TS | `WebAppWithAngular/ClientApp/src/app/counter/counter.component.ts` | `typescript-breakpoint` |
| C# and TS in one session | `WebAppWithAngular/ClientApp/src/app/fetch-data/fetch-data.component.ts` | `typescript-and-csharp-same-session` |
| Desktop | `WpfApp1/MainWindow.xaml.cs` | `attach-to-local-process` |
| File-based app | `SingleFileApp1.cs` | `file-based-app-session` |

# How to run the checklist

```
dotnet run --project ConsoleApp1                  # every scenario group, in checklist order
dotnet run --project ConsoleApp1 -- stepping      # a single group
dotnet run SingleFileApp1.cs                      # the file-based app
```

`ConsoleApp1` scenario groups: `breakpoints`, `stepping`, `evaluation`, `visualizers`,
`concurrency`, `exceptions`, `native`, `console`, `crossthread`.

The other projects are started from their Rider run configurations.

## Conventions used in the test projects

Every scenario in the code carries a stable name and instructions in comments:

* `// SCENARIO "name"` – the scenario name referenced by this checklist. Search the solution
  for the name to jump straight to the code.
* `// BREAKPOINT:` – where to put a breakpoint and of which kind.
* `// SETUP:` – debugger configuration to change before running.
* `// STEP OVER: / STEP INTO: / STEP OUT:` – the stepping action to perform.
* `// WATCH: / EVALUATE: / SET VALUE:` – the expression to use and the value to expect.
* `// EXCEPTION:` – the exception breakpoint to configure.
* `// EXPECTED:` – what confirms the feature works.

Each item below states **Where** (project → file → method / scenario name), **Setup**,
**Action** and **Expected**.

# Scope

## Basics

### Core actions

1. (Re)Run a debug session

   *Where:* `ConsoleApp1/Program.cs` → `Main` – breakpoint `session-start`.
   *Setup:* line breakpoint on the `Console.WriteLine("=== ConsoleApp1 debugger scenarios ===")` line.
   *Action:* Run with debugger, then Re-run.

   **Expected:** The application starts under the debugger and a new debug session becomes active.

2. Pause a debug session

   *Where:* `SingleFileApp1.cs` – scenario `file-based-app-session` (a long-running loop is
   easier to pause than a short one).
   *Action:* start it under the debugger, let it print a few lines, then Pause.

   **Expected:** Application execution is suspended and the current threads, stack frames, and variables become available.

3. Resume a debug session

   *Where:* same as above.

   **Expected:** Application execution continues from the current position.

4. Stop a debug session

   *Where:* same as above.

   **Expected:** The debug session terminates and the debugger UI returns to the non-debugging state.

Could be invoked from:

* Toolbar
* Debug tool window
* `Run` menu
* Shortcuts

Also run 1–4 once against `BlazorAppMultitarget` with the target framework switched from
`net10.0` to `net9.0` (scenario `multitarget-session` in `BlazorAppMultitarget/Program.cs`) to
cover the Multitarget requirement.

### Threads & Variables

1. Threads list

   *Where:* `ConsoleApp1/Scenarios/ConcurrencyScenarios.cs` → `Tick` – scenario
   `threads-and-parallel-stacks`.
   *Setup:* line breakpoint on the `Console.WriteLine($"threads: [{name}] tick {i}")` line, with
   the breakpoint suspend policy set to **All**.
   *Action:* run the `concurrency` group and switch between the listed threads.

   **Expected:** All active threads are displayed – `Worker-A`, `Worker-B`, `Worker-C` and the main
   thread. Selecting a thread updates the current execution context, and the available thread actions work correctly.

2. Stack Frames list

   *Where:* same scenario.
   *Action:* select the `Tick` frame, then the `Worker` frame.

   **Expected:** Stack frames are displayed in the correct order (`Tick` → `Worker` → thread start).
   Selecting a frame updates the editor position and variables for that frame.

3. Add a watch

   *Where:* `ConsoleApp1/Scenarios/EvaluationScenarios.cs` → `VariablesAndWatches` – scenario
   `variables-and-watches`.
   *Setup:* line breakpoint on `Console.WriteLine(Summarize(order));`.

    1. A regular watch – add `order.Total * 2`.

       **Expected:** The expression appears in the Watches view and shows its current value, `250.00`.

    2. An inlay watch – add an inlay watch for `order.Customer`.

       **Expected:** The watched value (`"Contoso"`) is displayed as an inlay in the editor and updates when execution context changes.

   **Expected:** The watch is added and its value is evaluated in the current debugging context.

4. Set a variable value

   *Where:* same scenario – set `order.IsPaid` to `true`.
   Desktop variant: `WpfApp1/MainWindow.xaml.cs` → `Divide_Click` – scenario
   `set-variable-value-desktop` (enter `10` and `0`, press **Div**, set `second` to `4`).

   **Expected:** The variable is updated to the entered value and the new value is used by the
   running application – the console prints `PAID` instead of `UNPAID`; the WPF window shows `2.5`
   instead of the divide-by-zero dialog.

5. Pin to top

   *Where:* same scenario – pin `Customer` inside `order`.

   **Expected:** The selected variable remains pinned at the top of the Variables view.

### Breakpoints

1. Set and hit breakpoints in user code

   *Where:* `ConsoleApp1/Scenarios/BreakpointScenarios.cs` → `LineAndInlineBreakpoints` – scenario
   `line-and-inline-breakpoint`.

    1. Line breakpoints – on `var sum = Square(a) + Square(b) + Square(a + b);`.

       **Expected:** Execution stops on the expected source line; `sum` is `38` after it completes.

    2. Inline breakpoints – on the `Square(b)` expression of the same line.

       **Expected:** Execution stops on the selected expression or statement within the source line,
       and the other two calls on that line do not suspend.

   Repeat 1.1 in a Razor `@code` block (`BlazorAppMultitarget/Components/Pages/Counter.razor` –
   scenario `razor-code-block-breakpoint`), in a CSHTML code block
   (`WebApplicationClassic/Views/Home/About.cshtml` – scenario `cshtml-code-block-breakpoint`) and
   in TypeScript (`WebAppWithAngular/ClientApp/src/app/counter/counter.component.ts` – scenario
   `typescript-breakpoint`).
   Repeat 1.2 in `BlazorAppMultitarget/Program.cs` – scenario `inline-breakpoint-aspnet` (three
   `app.Use…()` statements share one line).

   **Expected:** The breakpoint is created, enabled, and the debugger suspends execution when the breakpoint is reached.

2. Hit a user code exception

   *Where:* `ConsoleApp1/Scenarios/ExceptionScenarios.cs` → `ThrowUserException` – scenario
   `user-code-exception`. ASP.NET Core variant: `BlazorAppMultitarget/Program.cs` – scenario
   `startup-user-exception`.
   *Setup:* CLR Exception Breakpoint for `System.InvalidOperationException`, break on **Thrown**,
   **User code only** on.

   **Expected:** The debugger suspends on the exception according to the configured exception
   breakpoint settings and shows the relevant user-code location – the `throw` line, not the `catch`.

3. Hit an external code exception

   *Where:* `ConsoleApp1/Scenarios/ExceptionScenarios.cs` → `ExternalCodeException` – scenario
   `external-code-exception`.
   *Setup:* CLR Exception Breakpoint for `System.FormatException` with **User code only** off – the
   throw happens inside `System.Private.CoreLib` when `int.Parse("not-a-number")` runs.

   **Expected:** The debugger suspends on the external-code exception when the corresponding
   exception breakpoint is enabled and shows the relevant stack information (library frames plus
   `ExternalCodeException` as the caller).

4. Remove one breakpoint

   *Where:* any breakpoint from item 1.

   **Expected:** The breakpoint disappears from the editor and Breakpoints view and is no longer hit.

### Stepping

*Where (1–9):* `ConsoleApp1/Scenarios/SteppingScenarios.cs`, run with
`dotnet run --project ConsoleApp1 -- stepping`.

1. Step Over

   *Method:* `StepOverAndForceStepOver` – scenario `step-over-and-force-step-over`.
   *Setup:* line breakpoint on `var doubled = Double(value);` **and** on `return x * 2;` inside `Double`.
   *Action:* Step Over on the `var doubled = …` line.

   **Expected:** The current statement and its method calls are executed. Execution stops at the next statement in the current context, or at a breakpoint hit inside a called method – here, at the breakpoint inside `Double`.

2. Force Step Over

   *Method:* same, same breakpoints. Restart, then Force Step Over on the `var doubled = …` line.

   **Expected:** The current statement and its method calls are executed **without stopping at breakpoints inside the call chain**. Execution stops at the next statement in the current context, with `doubled == 42`.

3. Step Into

   *Method:* `StepIntoAndStepOut` – scenario `step-into-and-step-out`.
   *Setup:* line breakpoint on `var result = Outer(3);`.

   **Expected:** The debugger enters the called method (`Outer`, then `Inner`), and the Frames list grows with each step.

4. Smart Step Into

   *Method:* `SmartStepInto` – scenario `smart-step-into`.
   *Setup:* line breakpoint on `var report = Format(Describe(7), Describe(8));`.

   **Expected:** Available call targets are shown – `Format` and both `Describe` calls – and the debugger enters the selected target. Choosing the second `Describe` suspends with `n == 8`.

5. Step Out

   *Method:* `StepIntoAndStepOut`, from inside `Inner`.

   **Expected:** Execution continues until the current method returns and then suspends in the caller (`Outer`, then `StepIntoAndStepOut`). `result` is `36`.

6. Run to Cursor

   *Method:* `RunToCursorAndSkipToCursor` – scenario `run-to-cursor-and-skip-to-cursor`.
   *Setup:* line breakpoint on `int total = 0;`; caret on the `Console.WriteLine($"run-to-cursor: …")` line.

   **Expected:** Execution continues normally and suspends when the line at the cursor is reached, with `total == 45`.

7. Skip to Cursor

   *Method:* same. Restart, stop on `int total = 0;`, Step Over once, put the caret on the same
   `Console.WriteLine` line and Skip to Cursor.

   **Expected:** The execution position moves to the selected location without executing the skipped statements – the loop never runs and `total` is still `0`.

8. Stepping Time

   *Method:* `SteppingTime` – scenario `stepping-time`.
   *Setup:* line breakpoint on `var checksum = SlowCall();` (the call blocks ~750 ms).

   **Expected:** Stepping time information is displayed and updated after stepping operations.

9. Step through an external source code

   *Method:* `StepIntoExternalCode` – scenario `step-into-external-code`.
   *Setup:* in **Settings | Build, Execution, Deployment | Debugger**, allow stepping into external
   code / enable external source debug. Line breakpoint on `var joined = string.Join(", ", parts);`.

   **Expected:** The debugger opens the available external source (`String.Join`) and allows stepping through it. `joined` is `"alpha, beta, gamma"`.

### Evaluation

Evaluate a variable by hovering over the name of the variable in the editor window

*Where:* `ConsoleApp1/Scenarios/EvaluationScenarios.cs` → `VariablesAndWatches` – hover over `order`.

**Expected:** A debugger tooltip appears and shows the current value of the variable and can be expanded to its members.

**Mixed mode debug**

Debug managed and native code in the same session

*Where:* `ConsoleApp1/Scenarios/NativeInteropScenarios.cs` → `Run` – scenario `mixed-mode-debug`
(Windows x64 only).
*Setup:* enable native code debugging (mixed mode) in the `ConsoleApp1` run configuration; line
breakpoint on `var ticks = GetTickCount64();`.
*Action:* Step Into the `GetTickCount64` P/Invoke.

**Expected:** The debugger can suspend, inspect stacks, and step through supported managed and native frames within the same debug session – `kernel32!GetTickCount64` appears in the Frames list next to the managed frames.

## More

### Threads & Variables

1. Threads list with all the related actions

   *Where:* `ConsoleApp1/Scenarios/ConcurrencyScenarios.cs` → `Tick` – scenario
   `threads-and-parallel-stacks` (breakpoint suspend policy **All**).

    1. Freeze/Unfreeze Threads actions – freeze `Worker-B` and resume.

       **Expected:** A frozen thread does not continue execution when the application is resumed – only `Worker-A` and `Worker-C` keep printing. After unfreezing, the thread can continue execution.

    2. Navigate to Threads in Parallel Stacks

       **Expected:** The Parallel Stacks view opens and the selected thread is highlighted.

    3. Copy All Stacks

       **Expected:** Stack traces of all threads are copied to the clipboard.

    4. Show/Hide Threads

       **Expected:** Threads are shown or hidden according to the selected visibility option.

2. Stack Frames list with all the related actions

   *Where:* same scenario, unless stated otherwise.

    1. Copy Stack

       **Expected:** The currently selected stack trace is copied to the clipboard.

    2. Refresh Stack Frames

       **Expected:** The stack frames are refreshed and reflect the current execution state.

    3. Navigate to Modules View

       **Expected:** The Modules view opens and shows the module related to the selected stack frame.

    4. Show actions

        1. Frames from Libraries

           *Where:* `ConsoleApp1/Scenarios/ExceptionScenarios.cs` → `ExternalCodeException` (the
           stack contains `System.Private.CoreLib` frames).

           **Expected:** Library frames are shown or hidden in the stack according to the selected option.

        2. Async Internals

           *Where:* `ConsoleApp1/Scenarios/ConcurrencyScenarios.cs` → `BuildReportAsync` – scenario
           `async-stepping`.

           **Expected:** Internal async frames are shown or hidden according to the selected option.

        3. Method Return Value

           *Where:* `ConsoleApp1/Scenarios/EvaluationScenarios.cs` → `Label<T>` – scenario
           `generic-frame-info`. Step out and check the returned `"Int32:5"`.

           **Expected:** Method return values are shown or hidden according to the selected option.

        4. Method Parameters

           *Where:* same scenario – the frame shows `value: 5`.

           **Expected:** Method parameters are shown or hidden in the stack frame presentation.

        5. Method Generic Parameters

           *Where:* same scenario – the frame shows `Label<System.Int32>`.

           **Expected:** Generic method parameters are shown or hidden in the stack frame presentation.

        6. Raw Frame Presentation

           *Where:* same scenario.

           **Expected:** Stack frames switch between raw and regular presentation according to the selected option.

3. Add a watch

   *Where:* `ConsoleApp1/Scenarios/EvaluationScenarios.cs` → `VariablesAndWatches` – scenario
   `variables-and-watches`.

    1. From the Variables view – right-click `order.Total`.

       **Expected:** The selected variable or expression is added to Watches.

    2. From the Immediate window – type `order.Items.Count` and use **Add Watch**.

       **Expected:** The selected or entered expression can be added to Watches with the Add Watch button.

    3. From the editor – select `order.Customer` in the source and use the context menu.

       **Expected:** The selected expression is added to Watches from the editor context menu.

    4. Pause/Resume watch – pause the `order.Total * 2` watch, step, then resume it.

       **Expected:** A paused watch stops automatic evaluation. After resuming, its value is evaluated and updated again.

    5. Global watch (`[DebuggerGlobalWatch]`)

       *Where:* `BlazorAppMultitarget/Program.cs` – scenario `global-watch`, and the prepared
       attributes on `BlazorAppMultitarget/Components/Pages/Weather.razor` → `WeatherForecast`.
       *Setup:* uncomment the `JetBrains.Annotations` package reference in
       `BlazorAppMultitarget.csproj` and the attributed members.

       **Expected:** The annotated members appear in Watches automatically on every suspension where
       an instance is reachable, without being added by hand.

   **Expected:** The selected expression is added to Watches and evaluated in the current debugging context.

4. Copy name/value

   *Where:* same scenario – on `order.Items`.

   **Expected:** The selected variable name or value is copied to the clipboard correctly.

5. Inspect

   *Where:* same scenario – Inspect `order.Items`.

   **Expected:** The selected value opens in the inspection view with its available members and details.

6. Mark Object

   *Where:* same scenario – mark `order` as `theOrder`, then evaluate `theOrder_DebugLabel.Total`
   in the Immediate window.

   **Expected:** The selected object receives a debugger marker of a bright color and can be identified again while the object remains available.

### Breakpoints

1. Set and hit breakpoints in user code

   *Where:* `ConsoleApp1/Scenarios/BreakpointScenarios.cs`, run with
   `dotnet run --project ConsoleApp1 -- breakpoints`.

    1. Conditional breakpoints

       *Method:* `ConditionalBreakpoint` – scenario `conditional-breakpoint`.
       *Setup:* line breakpoint on the `Console.WriteLine` inside the loop with the condition `i == 7`.

       **Expected:** Execution suspends only when the configured condition evaluates to `true` – exactly once, with `i == 7`.

    2. Method breakpoints

       *Method:* `Visit` – scenario `method-breakpoint`.
       *Setup:* breakpoint on the **signature** line of `Visit`.

       **Expected:** Execution suspends when the configured method is entered – three times, with `label` equal to `"call-1"`, `"call-2"` and `"call-3"`.

    3. **\[Before release only\]** Data breakpoints

       *Method:* `DataBreakpoint` / `Tick.Bump` – scenario `data-breakpoint`.
       *Setup:* stop on `var tick = new Tick();`, step over once, then right-click `tick.Value` in
       Variables → **Add Data Breakpoint**, and remove the original line breakpoint.

       **Expected:** Execution suspends when the watched field is modified – five times inside `Tick.Bump()`.

       LIMITATIONS: works only for Windows x64, only for field \< 8 Byte, maximum 4 at once exist

   **Expected:** The configured breakpoint is created and suspends execution when its conditions are satisfied.

2. Restore a breakpoint

   *Where:* remove one of the breakpoints from item 1, then restore it from the Breakpoints dialog.

   **Expected:** A previously removed breakpoint is restored with the expected location and settings.

3. Remove all breakpoints

   **Expected:** All breakpoints are removed and no removed breakpoint can suspend execution.

4. Mute a breakpoint

   *Where:* the conditional breakpoint from 1.1 is a good candidate – muted, the ten loop
   iterations run without suspending.

   **Expected:** Muted breakpoints remain configured but do not suspend execution. Unmuting them makes them active again.

5. View Breakpoints and Exceptions dialogue

    1. .NET Line Breakpoints settings
    2. CLR Exception Breakpoints settings

   **Expected:** The dialog opens and displays the configured breakpoints grouped by type.

6. Bookmarks tool window

   *Where:* bookmark a few scenario lines in `ConsoleApp1/Scenarios/`.

   **Expected:** Debug-related source locations can be opened from the Bookmarks tool window and navigation goes to the correct file and line.

### Stepping

Step through user async code

*Where:* `ConsoleApp1/Scenarios/ConcurrencyScenarios.cs` → `BuildReportAsync` – scenario
`async-stepping`. Razor variant: `BlazorAppMultitarget/Components/Pages/Weather.razor` →
`OnInitializedAsync` – scenario `razor-async-stepping`.
*Setup:* line breakpoint on `var raw = await LoadAsync(id);`.
*Action:* Step Over across both awaits, then restart and Step Into / Step Out of `LoadAsync`.

**Expected:** Stepping follows the logical user-code flow across `await` boundaries and stops at the expected source locations – never inside compiler-generated `MoveNext` frames. `raw` is `"value-7"`, `transformed` is `"VALUE-7"`.

### Evaluation

1. Evaluate variables and expressions via the Immediate window

   *Where:* `ConsoleApp1/Scenarios/EvaluationScenarios.cs` → `ImmediateWindowAndSyntheticVariable` –
   scenario `immediate-window-and-synthetic-variable`.
   *Action:* evaluate `basket.Count` (→ `4`), `basket[2].ToUpper()` (→ `"CHERRY"`),
   `basket.Sum(x => x.Length)` (→ `21`) and the invalid `basket.NoSuchMember`.

   **Expected:** Valid expressions are evaluated and the correct result is displayed. Evaluation errors are reported clearly.

2. Evaluate variables and expressions via the Evaluation dialogue

   *Where:* same scenario, same expressions.

   **Expected:** Valid expressions are evaluated and the correct result is displayed. Evaluation errors are reported clearly.

3. Create a synthetic variable

   *Where:* same scenario – run `var doubled = basket.Count * 2;` in the Immediate window.

   **Expected:** The synthetic variable is created, displayed in the debugger, and contains the expected value (`8`).

4. Use string visualizer (HTML, XML, JSON)

   *Where:* `ConsoleApp1/Scenarios/VisualizerScenarios.cs` → `StringVisualizers` – scenario
   `string-visualizer`; the locals `html`, `xml` and `json`.

   **Expected:** The full string value opens in the visualizer and is displayed correctly with the matching highlighting.

5. Use collection visualizer

   *Where:* `ConsoleApp1/Scenarios/VisualizerScenarios.cs` → `CollectionVisualizer` – scenario
   `collection-visualizer`; the local `people` holds 50 rows.

   **Expected:** Collection items are displayed correctly in a DataGrip-like table and can be inspected using the available visualizer controls.

6. Use LINQ visualizer

   *Where:* `ConsoleApp1/Scenarios/VisualizerScenarios.cs` → `LinqVisualizer` – scenario
   `linq-visualizer`; the local `query`.

   **Expected:** The LINQ expression is visualized and its result matches the evaluated expression – `["Ada", "Alan", "Grace", "Margaret"]`.

7. Use predictive debugger with boolean operations

   *Where:* `ConsoleApp1/Scenarios/EvaluationScenarios.cs` → `PredictiveBooleans` – scenario
   `predictive-boolean`. Desktop variant: `WpfApp1/MainWindow.xaml.cs` → `Divide_Click`.
   *Setup:* line breakpoint on `if (hasAccess && !isExpired)`; then flip `isExpired` to `true`.

   **Expected:** The debugger correctly predicts the result of supported Boolean expressions and displays the predicted execution outcome, and the prediction updates when an operand is changed.

8. Use predictive debugger with loops

   *Where:* `ConsoleApp1/Scenarios/EvaluationScenarios.cs` → `PredictiveLoop` – scenario
   `predictive-loop`.
   *Setup:* line breakpoint on the `foreach` line; `matched` is `3` after the loop.

   **Expected:** The debugger correctly displays supported predicted values or execution flow for loop iterations.

9. Evaluate a property with a cross-thread dependency

   *Where:* `ConsoleApp1/Program.cs` → `CrossThreadDependency` / `SomeClass.Name` – scenario
   `cross-thread-evaluation`.
   *Action:* stop on the `Console.WriteLine` in `CrossThreadDependency` and expand `a.Name`.

   **Expected:** Because the getter calls `Debugger.NotifyOfCrossThreadDependency()`, the debugger does not evaluate the property silently – it offers an explicit "load value" action, and the value is produced only after it is confirmed.

### Attach/Detach

1. Attach to a local process

   *Where:* `WpfApp1/MainWindow.xaml.cs` → `TryGetNumbers` – scenario `attach-to-local-process`,
   or `SingleFileApp1.cs`.
   *Setup:* start the app **without** the debugger, then `Run | Attach to Process` and set the
   breakpoint after attaching.

   **Expected:** The debugger attaches to the selected process, allows debugging it, and detaches or stops without affecting unrelated processes, behaves the same way as the usual debugger.

2. **\[Before release only\]** Attach to a remote process - see [RIDER-A-1553](https://youtrack.jetbrains.com/articles/RIDER-A-1553/Remote-Debug-Checklist) for more details

### Console

*Where:* `ConsoleApp1/Scenarios/ConsoleScenarios.cs` → `Run` – scenario `console-output`
(`dotnet run --project ConsoleApp1 -- console`). Long-running variant: `SingleFileApp1.cs` –
scenario `console-during-pause-resume`.

1. Application standard output

   *Expected values:* `console-out: line N` for N = 2, 4, 6, 8, 10.

   **Expected:** Application output is displayed in the debugger console without missing or duplicated messages.

2. Application standard errors

   *Expected values:* `console-err: line N` for N = 1, 3, 5, 7, 9, highlighted differently from stdout.

   **Expected:** Error output is displayed correctly in the debugger console.

3. Console.WriteLine() output

   *Where:* also `BlazorAppMultitarget/Program.cs` – scenario `startup-console-output` prints `0`…`4`.

   **Expected:** The text written by `Console.WriteLine()` appears in the application console with the expected content and order.

4. Console during Pause/Resume

   *Action:* the loop runs for ~3 s; Pause in the middle, then Resume.

   **Expected:** Console output continues correctly after the application is resumed – it stops while suspended and continues from the next number, with no gaps or repeats.

### Debug Output

Check debugger output

*Where:* `BlazorAppMultitarget` – an ASP.NET Core app loads many assemblies and starts several
threads, so the Debug Output view fills up during startup.

**Expected:** Debug messages produced by the application are displayed in the Debug Output view.

Example of such an output:

```
Loaded Assembly 'C:\Program Files\dotnet\shared\Microsoft.NETCore.App\10.0.10\System.Net.WebSockets.dll'
Loading module C:\Program Files\dotnet\shared\Microsoft.NETCore.App\10.0.10\System.Net.WebSockets.dll in application domain 1:clrhost
Pdb file for assembly C:\Program Files\dotnet\shared\Microsoft.NETCore.App\10.0.10\System.Net.WebSockets.dll was not found or failed to read
Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionMiddleware: Warning: Failed to determine the https port for redirect.
Loaded Assembly 'C:\Program Files\dotnet\shared\Microsoft.AspNetCore.App\10.0.10\Microsoft.AspNetCore.Html.Abstractions.dll'
Loading module C:\Program Files\dotnet\shared\Microsoft.AspNetCore.App\10.0.10\Microsoft.AspNetCore.Html.Abstractions.dll in application domain 1:clrhost
Pdb file for assembly C:\Program Files\dotnet\shared\Microsoft.AspNetCore.App\10.0.10\Microsoft.AspNetCore.Html.Abstractions.dll was not found or failed to read
Started Thread 2492
Started Thread 26584
Started Thread 30536
```

### Parallel Stacks

*Where:* `ConsoleApp1/Scenarios/ConcurrencyScenarios.cs` → `Tick` – scenario
`threads-and-parallel-stacks` (breakpoint suspend policy **All**, three named worker threads).

1. Open Parallel Stacks during a paused session

   **Expected:** Active thread or task stacks are displayed and represent the current execution state – one branch per worker, sharing the `Worker`/`Tick` frames.

2. Select a stack/frame

   **Expected:** Selecting an item updates the current debugger context and navigates to the corresponding source location.

3. Resume and pause again

   **Expected:** Parallel Stacks refreshes and reflects the new execution state.

4. Freeze/Unfreeze Threads actions

   **Expected:** A frozen thread does not continue execution when the application is resumed. After unfreezing, the thread can continue execution.

5. Show actions

    1. Frames from Libraries

       **Expected:** Library frames are shown or hidden in Parallel Stacks according to the selected option.

    2. Async Internals

       *Where:* switch to scenario `tasks-view` in the same file for a stack that actually contains
       async frames.

       **Expected:** Internal async frames are shown or hidden according to the selected option.

    3. Raw Frame Presentation

       **Expected:** Stack frames switch between raw and regular presentation according to the selected option.

### Memory

*Where:* `ConsoleApp1/Scenarios/VisualizerScenarios.cs` → `CollectionVisualizer` – 50 `Person`
objects and a `List<Person>` are alive on the managed heap when the breakpoint is hit.

**Expected:** The Memory view opens and displays managed heap data.

### Modules

*Where:* `BlazorAppMultitarget/Program.cs` – the `aspnet-pipeline` breakpoint (an ASP.NET Core host
loads many modules). `ConsoleApp1` works too, with a much shorter module list.

1. Open Modules during a debug session

   **Expected:** Loaded modules are displayed for the debugged process.

2. Inspect module information

   **Expected:** Module name, path, symbols status, and other available module information are displayed correctly.

3. Open or navigate to a module-related source when available

   **Expected:** The debugger opens the corresponding source or decompiled code when supported.

### Tasks

*Where:* `ConsoleApp1/Scenarios/ConcurrencyScenarios.cs` → `WorkAsync` – scenario `tasks-view`.
*Setup:* line breakpoint on `await Task.Delay(150 * n);` with the suspend policy set to **All**, so
several of the four tasks are in flight at once.

1. Open Tasks during a paused async application

   **Expected:** Available .NET tasks are displayed with their current states.

2. Inspect a task

   **Expected:** Task details and related stack information are displayed correctly.

3. Select a task-related frame

   **Expected:** The debugger navigates to the corresponding source location and updates the current context – `WorkAsync`, with a different `n` per task.

4. Resume and pause again

   **Expected:** The Tasks view refreshes and reflects the current task states.

5. Switch between the Table and Graph views

   **Expected:**

    * Tables view displays all tasks in table format
    * Graphs view shows only active tasks as a graph

### [Before release only] JIT-debugger

*Where:* `ConsoleApp1/Scenarios/ExceptionScenarios.cs` → `JitDebugger` – scenario `jit-debugger`;
`WpfApp1/MainWindow.xaml.cs` constructor – scenario `jit-debugger-wpf`;
`BlazorAppMultitarget/Program.cs` – scenario `jit-debugger`. Each has both calls prepared as
comments; uncomment one and start the app **without** the debugger.

1. Put Debugger.Launch() in C# code

   **Expected:** The configured JIT debugger is started or activated and attaches to the process at the requested location.

2. Put Debugger.Break() in C# code

   **Expected:** The debugger suspends the process at the `Debugger.Break()` call and shows the correct source location and stack.
