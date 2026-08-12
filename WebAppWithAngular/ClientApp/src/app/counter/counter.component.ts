import { Component } from '@angular/core';

@Component({
  selector: 'app-counter-component',
  templateUrl: './counter.component.html'
})
export class CounterComponent {
  // SCENARIO "typescript-breakpoint" (JS/TS language coverage)
  //   README -> Basics -> Core actions, Breakpoints 1.1, Stepping 1, Threads & Variables 3-4.
  // SETUP: start WebAppWithAngular under the debugger (enable `with JavaScript debugger` option in the corresponding Run Configuration) so the SPA proxy launches the
  //   browser with the JS debugger attached, then open /counter.
  // BREAKPOINT: on the first 'this.currentCount = ...' line below.
  // ACTION: click "Increment".
  // EXPECTED: the debugger suspends in this .ts file, 'this.currentCount' is 0 in
  //   Variables, and Step Over advances one assignment at a time.
  // WATCH: add 'this.currentCount' and verify it reads 0, 50, 100, 150 as you step.
  // EXPECTED after Resume: the page shows 200.
  public currentCount = 0;

  public incrementCounter() {
    this.currentCount = this.currentCount + 50;
    this.currentCount = this.currentCount + 50;
    this.currentCount = this.currentCount + 50;
    this.currentCount = this.currentCount + 50;
  }
}
