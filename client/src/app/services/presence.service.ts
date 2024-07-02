import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { User } from '../models/user';
import { BehaviorSubject, take } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  ubUrl = environment.hubUrl;
  private hubConnection?: HubConnection
  private onlinUsersSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlinUsersSource.asObservable();

  constructor(
    private toastr: ToastrService,
    private router: Router
  ) { }

  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.ubUrl + 'presence', {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch(err => console.log(err));

    this.hubConnection.on('UserIsOnline', username => {
      this.onlineUsers$.pipe(take(1)).subscribe({
        next: usernames => {
          this.onlinUsersSource.next([...usernames, username]);
        }
      })
    });

    this.hubConnection.on('UserIsOffline', username => {
      this.onlineUsers$.pipe(take(1)).subscribe({
        next: usernames => {
          this.onlinUsersSource.next(usernames.filter(x => x !== username));
        }
      })
    });

    this.hubConnection.on('GetOnlineUsers', username => {
      this.onlinUsersSource.next(username);
    });

    this.hubConnection.on('NewMessageReceived', ({ username, knownAs }) => {
      this.toastr.info(knownAs + ' has sent you a new message! Click to read.')
        .onTap.pipe(take(1)).subscribe({
          next: () => this.router.navigateByUrl('/members/' + username + '?tab=Messages')
        })
    });
  }

  stopHubConnection() {
    this.hubConnection?.stop().catch(error => console.log(error));
  }
}
