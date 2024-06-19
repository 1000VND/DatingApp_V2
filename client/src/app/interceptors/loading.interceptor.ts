import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, catchError, delay, finalize, of, switchMap, throwError } from 'rxjs';
import { BusyService } from '../services/busy.service';

@Injectable()
export class LoadingInterceptor implements HttpInterceptor {
  constructor(
    private busyService: BusyService
  ) { }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    this.busyService.busy();
    const startTime = Date.now();

    return next.handle(request).pipe(
      switchMap(event => {
        const endTime = Date.now();
        const requestTime = endTime - startTime;
        return of(event).pipe(delay(requestTime));
      }),
      catchError((error) => {
        this.busyService.idle();
        return throwError(error);
      }),
      finalize(() => {
        this.busyService.idle();
      })
    )
  }
}
