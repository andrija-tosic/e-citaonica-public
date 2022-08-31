import {
  HttpEvent,
  HttpEventType,
  HttpResponse,
  HttpProgressEvent,
} from '@angular/common/http'

export interface Upload {
  progress: number
  state: 'PENDING' | 'IN_PROGRESS' | 'DONE',
  path?: string
}

export function isHttpResponse(event: HttpEvent<any>): boolean {
  return event.type === HttpEventType.Response
}

export function isHttpProgressEvent(
  event: HttpEvent<any>
): boolean {
  return (
    event.type === HttpEventType.DownloadProgress ||
    event.type === HttpEventType.UploadProgress
  )
}

export function calculateState(upload: Upload, ev: HttpEvent<any>): Upload {
  if (isHttpProgressEvent(ev)) {
    const pev = ev as HttpProgressEvent;
    console.log(pev);
    return {
      progress: pev.total
        ? Math.round((100 * pev.loaded) / pev.total)
        : upload.progress,
      state: 'IN_PROGRESS',
    }
  }
  if (isHttpResponse(ev)) {
    const evr = ev as HttpResponse<any>;
    return {
      progress: 100,
      state: 'DONE',
      path: evr.body.path
    }
  }
  return upload
}