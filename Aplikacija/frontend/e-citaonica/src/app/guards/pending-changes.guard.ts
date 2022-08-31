import { Observable } from 'rxjs';
import { CanDeactivate } from '@angular/router';
import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { DeleteConfirmationDialogComponent } from '../components/delete-confirmation-dialog/delete-confirmation-dialog.component';
import { BrowserRefreshService } from '../services/browser-refresh.service';

export interface ComponentCanDeactivate {
    canDeactivate: () => boolean | Observable<boolean>;
}

@Injectable()
export class PendingChangesGuard implements CanDeactivate<ComponentCanDeactivate> {
    constructor(private dialog: MatDialog, private browserRefresh: BrowserRefreshService) { }

    canDeactivate(component: ComponentCanDeactivate): boolean | Observable<boolean> {
        // if there are no pending changes, just allow deactivation; else confirm first
        return component.canDeactivate() ?
            true :
            this.dialog.open(DeleteConfirmationDialogComponent,
                {
                    data:
                    {
                        title: "Niste sačuvali promene. Ako napustite stranicu, promene će biti poništene. Sigurno napustiti stranicu?"
                    }
                }
            ).afterClosed();
    }
}
