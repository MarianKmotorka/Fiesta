//k6 run script.js

import http from 'k6/http';
import { sleep } from 'k6';

export let options = {
    vus: 10,
    duration: '30s',
  };

export default function () {
  http.get('https://fiesta-api.azurewebsites.net/health');
  sleep(1);
}
