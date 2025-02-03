import { TestBed } from '@angular/core/testing';

import { UtilitiesServiceService } from './utilities-service.service';

describe('UtilitiesServiceService', () => {
  let service: UtilitiesServiceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(UtilitiesServiceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
