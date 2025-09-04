import { useState } from "react"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Checkbox } from "@/components/ui/checkbox"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { CodeExample } from "@/components/CodeExample"
import { UserPlus } from "lucide-react"

export default function Signup() {
  const [email, setEmail] = useState("")
  const [password, setPassword] = useState("")
  const [confirmPassword, setConfirmPassword] = useState("")
  const [keepSignedIn, setKeepSignedIn] = useState(false)

  const reactCode = `// Simple Signup Component
import { useState } from 'react'
import { useAuth } from '@nauth/react'

export function SignupForm() {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [keepSignedIn, setKeepSignedIn] = useState(false)
  const { register, loading, error } = useAuth()

  const handleSubmit = async (e) => {
    e.preventDefault()
    
    if (password !== confirmPassword) {
      alert('Passwords do not match')
      return
    }

    try {
      await register({
        email,
        password,
        confirmPassword,
        rememberMe: keepSignedIn
      })
    } catch (err) {
      console.error('Registration failed:', err)
    }
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <div>
        <label htmlFor="email">Email</label>
        <input
          id="email"
          type="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
        />
      </div>
      <div>
        <label htmlFor="password">Password</label>
        <input
          id="password"
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          minLength={8}
          required
        />
      </div>
      <div>
        <label htmlFor="confirmPassword">Confirm Password</label>
        <input
          id="confirmPassword"
          type="password"
          value={confirmPassword}
          onChange={(e) => setConfirmPassword(e.target.value)}
          required
        />
      </div>
      <div>
        <input
          id="keep-signed-in"
          type="checkbox"
          checked={keepSignedIn}
          onChange={(e) => setKeepSignedIn(e.target.checked)}
        />
        <label htmlFor="keep-signed-in">Keep me signed in</label>
      </div>
      <button type="submit" disabled={loading}>
        {loading ? 'Creating account...' : 'Create Account'}
      </button>
      {error && <p className="error">{error}</p>}
    </form>
  )
}`

  const dotnetCode = `// RegisterController.cs
using Microsoft.AspNetCore.Mvc;
using NAuth.Services;
using NAuth.Models;

[ApiController]
[Route("api/[controller]")]
public class RegisterController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IPasswordValidator _passwordValidator;
    private readonly ILogger<RegisterController> _logger;

    public RegisterController(
        IAuthService authService, 
        IPasswordValidator passwordValidator,
        ILogger<RegisterController> logger)
    {
        _authService = authService;
        _passwordValidator = passwordValidator;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var validationResult = await ValidateRegistrationAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var user = new User
            {
                Email = request.Email,
                CreatedAt = DateTime.UtcNow,
                IsEmailVerified = false
            };

            var result = await _authService.CreateUserAsync(user, request.Password);
            
            if (result.Success)
            {
                // Send email verification
                await _authService.SendEmailVerificationAsync(user);
                
                // Generate token if remember me is checked
                if (request.RememberMe)
                {
                    var token = await _authService.GenerateTokenAsync(result.User);
                    return Ok(new RegisterResponse
                    {
                        Message = "Account created successfully",
                        Token = token,
                        User = result.User
                    });
                }

                return Ok(new { message = "Account created successfully. Please verify your email." });
            }

            return BadRequest(new { message = result.ErrorMessage });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Registration failed for email: {Email}", request.Email);
            return StatusCode(500, new { message = "An error occurred during registration" });
        }
    }

    private async Task<ValidationResult> ValidateRegistrationAsync(RegisterRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrEmpty(request.Email))
            errors.Add("Email is required");
        else if (!IsValidEmail(request.Email))
            errors.Add("Invalid email format");
        else if (await _authService.EmailExistsAsync(request.Email))
            errors.Add("Email already exists");

        if (string.IsNullOrEmpty(request.Password))
            errors.Add("Password is required");
        else
        {
            var passwordValidation = _passwordValidator.Validate(request.Password);
            if (!passwordValidation.IsValid)
                errors.AddRange(passwordValidation.Errors);
        }

        if (request.Password != request.ConfirmPassword)
            errors.Add("Passwords do not match");

        return new ValidationResult { IsValid = !errors.Any(), Errors = errors };
    }
}

public class RegisterRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public bool RememberMe { get; set; }
}`

  return (
    <div className="container mx-auto max-w-6xl py-8 px-4">
      <div className="mb-8">
        <h1 className="text-3xl font-bold tracking-tight mb-2">Simple Signup</h1>
        <p className="text-muted-foreground">
          Quick and easy user registration with essential fields
        </p>
      </div>

      <div className="grid gap-8 lg:grid-cols-2">
        {/* Signup Form */}
        <div>
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <UserPlus className="h-5 w-5" />
                Create Account
              </CardTitle>
              <CardDescription>
                Sign up for a new account with just email and password
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="email">Email</Label>
                <Input
                  id="email"
                  type="email"
                  placeholder="john@example.com"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="password">Password</Label>
                <Input
                  id="password"
                  type="password"
                  placeholder="••••••••"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="confirm-password">Confirm Password</Label>
                <Input
                  id="confirm-password"
                  type="password"
                  placeholder="••••••••"
                  value={confirmPassword}
                  onChange={(e) => setConfirmPassword(e.target.value)}
                />
              </div>
              <div className="flex items-center space-x-2">
                <Checkbox
                  id="keep-signed-in"
                  checked={keepSignedIn}
                  onCheckedChange={(checked) => setKeepSignedIn(checked as boolean)}
                />
                <Label 
                  htmlFor="keep-signed-in" 
                  className="text-sm font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70"
                >
                  Keep me signed in
                </Label>
              </div>
              <Button className="w-full bg-gradient-to-r from-brand-primary to-brand-secondary hover:shadow-brand transition-all duration-300">
                Create Account
              </Button>
            </CardContent>
          </Card>
        </div>

        {/* Code Examples */}
        <div>
          <CodeExample
            reactCode={reactCode}
            dotnetCode={dotnetCode}
            title="Simple Registration Implementation"
          />
        </div>
      </div>
    </div>
  )
}