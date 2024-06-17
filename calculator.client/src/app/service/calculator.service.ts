import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";

@Injectable({ providedIn: 'root' })
export class CalculatorService {
    constructor(private http: HttpClient) { }

    getCalculation(bla: any) : Observable<any> {
        return this.http.get(`/calculator`, { params: {calculation: bla}  });
    }

    getHistory() {
        return this.http.get('/history');
    }

    clearHistory() {
        return this.http.get('/history/clear');
    }
}