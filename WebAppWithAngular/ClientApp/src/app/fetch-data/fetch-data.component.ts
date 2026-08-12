import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html'
})
export class FetchDataComponent {
  public forecasts: WeatherForecast[] = [];

  // SCENARIO "typescript-and-csharp-same-session" - README -> Basics -> Breakpoints 1.1.
  // BREAKPOINT: on 'this.forecasts = result;' below, together with the C# breakpoint in
  //   Controllers/WeatherForecastController.Get().
  // ACTION: enable `with JavaScript debugger` option in the corresponding Run Configuration and open /fetch-data in the browser.
  // EXPECTED: the C# breakpoint is hit first, then this TypeScript breakpoint, in the same
  //   debug session. 'result' holds the 5 forecasts returned by the controller.
  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<WeatherForecast[]>(baseUrl + 'weatherforecast').subscribe(result => {
      this.forecasts = result;
    }, error => console.error(error));
  }
}

interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}
