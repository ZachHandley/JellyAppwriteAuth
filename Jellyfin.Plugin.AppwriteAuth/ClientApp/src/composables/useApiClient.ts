export interface RequestOptions extends Omit<RequestInit, 'body'> {
  body?: unknown;
}

export interface ApiResponse<T> {
  data: T | null;
  error: string | null;
  loading: boolean;
}

export function useApiClient() {
  const getAuthToken = (): string => {
    if (!window.ApiClient) {
      throw new Error('Jellyfin ApiClient not available');
    }

    // Try accessToken() method first, fallback to _accessToken property
    if (typeof window.ApiClient.accessToken === 'function') {
      return window.ApiClient.accessToken();
    }

    if (window.ApiClient._accessToken) {
      return window.ApiClient._accessToken;
    }

    throw new Error('Unable to retrieve authentication token');
  };

  const getBaseUrl = (): string => {
    if (!window.ApiClient) {
      throw new Error('Jellyfin ApiClient not available');
    }

    // Use getUrl method to construct proper base URL
    if (typeof window.ApiClient.getUrl === 'function') {
      return window.ApiClient.getUrl('').replace(/\/$/, '');
    }

    // Fallback to serverAddress
    if (window.ApiClient.serverAddress) {
      return window.ApiClient.serverAddress();
    }

    throw new Error('Unable to retrieve server URL');
  };

  const request = async <T>(
    path: string,
    options: RequestOptions = {}
  ): Promise<T> => {
    const token = getAuthToken();
    const baseUrl = getBaseUrl();
    const url = `${baseUrl}${path}`;

    const headers: HeadersInit = {
      'X-Emby-Token': token,
      'Content-Type': 'application/json',
      ...options.headers,
    };

    const fetchOptions: RequestInit = {
      method: options.method,
      headers,
      credentials: options.credentials,
      cache: options.cache,
      redirect: options.redirect,
      referrer: options.referrer,
      referrerPolicy: options.referrerPolicy,
      integrity: options.integrity,
      keepalive: options.keepalive,
      signal: options.signal,
      mode: options.mode,
    };

    // Convert body to JSON if it's an object
    if (options.body && typeof options.body === 'object') {
      fetchOptions.body = JSON.stringify(options.body);
    }

    const response = await fetch(url, fetchOptions);

    if (!response.ok) {
      const errorText = await response.text().catch(() => 'Unknown error');
      throw new Error(`API request failed: ${response.status} - ${errorText}`);
    }

    // Handle empty responses
    const contentType = response.headers.get('content-type');
    if (!contentType || !contentType.includes('application/json')) {
      return {} as T;
    }

    return response.json();
  };

  const get = async <T>(path: string): Promise<T> => {
    return request<T>(path, { method: 'GET' });
  };

  const post = async <T>(path: string, body?: unknown): Promise<T> => {
    return request<T>(path, { method: 'POST', body });
  };

  const put = async <T>(path: string, body?: unknown): Promise<T> => {
    return request<T>(path, { method: 'PUT', body });
  };

  const del = async <T>(path: string): Promise<T> => {
    return request<T>(path, { method: 'DELETE' });
  };

  return {
    request,
    get,
    post,
    put,
    delete: del,
    getAuthToken,
    getBaseUrl,
  };
}
