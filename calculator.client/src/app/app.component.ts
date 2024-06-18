import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { CalculatorService } from './service/calculator.service';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  output = "0";
  history: Array<string> = [];
  response: any;
  show = false;

  constructor(private calcService: CalculatorService) {}

  ngOnInit() {
    this.clearHistory();
    this.getHistory();
  }

  add(input: any, str?: string) {
    if (str != "history") {
      if (this.output === "0" || this.output == "∞" || this.output == "Invalid input" || this.output == "NaN")
        this.output = input.toString();
      else
        this.output += input.toString();
    } else {
      this.output = input.split('=')[0].trim()
    }
    
  }

  calculate() {
    if (this.output !== "") { 
      this.calcService.getCalculation(this.output).subscribe(
        (data)  => {
          this.output = data[0];
        })
        this.getHistory();
      }
  }

  clear() {
    this.output = "0";
    this.clearHistory();
  }

  backspace() {
    if (this.output == "Invalid input") 
      this.output = "0"
    else
      this.output = this.output.substring(0, this.output.length-1)
  }

  getHistory() {
    this.history = [];
    this.calcService.getHistory().subscribe(
      (data) => {
        this.response = data
        this.response.forEach((element: string) => {
          this.history.push(element);
        });
      }
    )
  }

  clearHistory() {
    this.calcService.clearHistory().subscribe();
    this.history = [];
  }

  showData() {
    this.show = !this.show
  }
}
