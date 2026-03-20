import { JwtPayload as JwtDecodePayload } from 'jwt-decode';

export interface UserDto {
  readonly id: number;
  readonly email: string;
  readonly displayName: string;
  readonly avatarUrl: string | null;
  readonly provider: string;
  readonly createdAt: string;
}

export interface TokenResponse {
  readonly accessToken: string;
  readonly refreshToken: string;
  readonly expiresIn: number;
}

export interface JwtPayload extends JwtDecodePayload {
  readonly email: string;
  readonly name: string;
  readonly avatar_url?: string;
  readonly provider: string;
  readonly created_at?: string;
}
