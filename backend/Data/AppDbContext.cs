using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Utilizador> Utilizadores {get; set;}

    public DbSet<Produto> Produtos { get; set; }

    public DbSet<Supermercado> Supermercados { get; set; }

    public DbSet<ProdutoSupermercado> ProdutosSupermercados { get; set; }

    public DbSet<Receita> Receitas { get; set; }

    public DbSet<ProdutoReceita> ProdutosReceitas { get; set; }

    public DbSet<Relatorio> Relatorios { get; set; }

    public DbSet<RelatorioProduto> RelatoriosProdutos { get; set; }

    public DbSet<RelatorioReceita> RelatoriosReceitas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Relatorio - Utilizador
        modelBuilder.Entity<Relatorio>()
            .HasOne<Utilizador>()
            .WithMany()
            .HasForeignKey(r => r.UtilizadorId);

        //ProdutoSupermercado - Produto
        modelBuilder.Entity<ProdutoSupermercado>()
            .HasOne(ps => ps.Produto)
            .WithMany(p => p.ProdutosSupermercados)
            .HasForeignKey(ps => ps.ProdutoId);

        //ProdutoSupermercado - Supermercado
        modelBuilder.Entity<ProdutoSupermercado>()
            .HasOne(ps => ps.Supermercado)
            .WithMany(s => s.ProdutosSupermercados)
            .HasForeignKey(ps => ps.SupermercadoId);

        // ProdutoReceita
        modelBuilder.Entity<ProdutoReceita>()
            .HasKey(pr => new { pr.ProdutoId, pr.ReceitaId });

        // ProdutoReceita - Produto
        modelBuilder.Entity<ProdutoReceita>()
            .HasOne<Produto>()
            .WithMany()
            .HasForeignKey(pr => pr.ProdutoId);

        // ProdutoReceita - Receita
        modelBuilder.Entity<ProdutoReceita>()
            .HasOne<Receita>()
            .WithMany()
            .HasForeignKey(pr => pr.ReceitaId);

        // RelatorioProduto
        modelBuilder.Entity<RelatorioProduto>()
         .HasKey(rp => new { rp.RelatorioId, rp.ProdutoId });

        // RelatorioProduto - Relatorio
        modelBuilder.Entity<RelatorioProduto>()
            .HasOne<Relatorio>()
            .WithMany()
            .HasForeignKey(rp => rp.RelatorioId);

        // RelatorioProduto - Produto
        modelBuilder.Entity<RelatorioProduto>()
            .HasOne<Produto>()
            .WithMany()
            .HasForeignKey(rp => rp.ProdutoId);

        // RelatorioReceita
        modelBuilder.Entity<RelatorioReceita>()
            .HasKey(rr => new { rr.RelatorioId, rr.ReceitaId });

        // RelatorioReceita - Relatorio
        modelBuilder.Entity<RelatorioReceita>()
            .HasOne<Relatorio>()
            .WithMany()
            .HasForeignKey(rr => rr.RelatorioId);

        // RelatorioReceita - Receita
        modelBuilder.Entity<RelatorioReceita>()
            .HasOne<Receita>()
            .WithMany()
            .HasForeignKey(rr => rr.ReceitaId);
                
        
    }


}